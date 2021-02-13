using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Pilchard123.ODSAPI.APIResponses;
using Pilchard123.ODSAPI.APIResults;

namespace Pilchard123.ODSAPI
{
    public class ODSClient
    {
        private const string BaseAddress = "https://directory.spineservices.nhs.uk/ORD/2-0-0";
        private const int PageSize = 1000;

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="ODSClient"/> class using the supplied <see cref="HttpClient"/> for making requests.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient" /> to be used for making requests to the <see href="https://digital.nhs.uk/services/organisation-data-service/apis-for-the-organisation-data-service">NHS ODS API</see>.
        /// Disposing of the <see cref="HttpClient"/> is the responsibility of the user.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
        public ODSClient(HttpClient httpClient)
        {
            if (httpClient is null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            _httpClient = httpClient;
        }

        /// <summary>
        /// Returns the codes of organisations updated on or after <paramref name="lastChangeDate"/>, in no particular order.
        /// Uses the <see href="https://digital.nhs.uk/services/organisation-data-service/guidance-for-developers/sync-endpoint">sync endpoint</see> to fetch the codes.
        /// </summary>
        /// <param name="lastChangeDate">The date from which to search. May not be more than 185 days in the past.</param>
        /// <returns>The codes of organisations updated on or after <paramref name="lastChangeDate"/>, in no particular order.</returns>
        /// <exception cref="APIException">Thrown when the API returns a non-success status code</exception>
        public async Task<IEnumerable<string>> GetUpdatedOrganisationCodesAsync(
            DateTime lastChangeDate
        )
        {
            var requestUri = $"{BaseAddress}/sync?LastChangeDate={lastChangeDate:yyyy-MM-dd}";
            var result = await _httpClient.GetAsync(
                requestUri: requestUri
            );

            await CheckErrors(result);

            using (var resStream = await result.Content.ReadAsStreamAsync())
            {
                var typedResult = await JsonSerializer.DeserializeAsync<SynchroniseResponse>(resStream);
                return typedResult.Organisations.Select(o => o.OrgLink.Split('/').Last());
            }
        }

        public async Task<IEnumerable<OrganisationSummary>> Search(string name = null, string postcode = null, DateTime? lastChangeDate = null, OrganisationStatus? status = null,
                                               string primaryRoleId = null, string nonPrimaryRoleId = null, IEnumerable<string> roles = null, string recordClass = null,
                                               int? timeout = null, CancellationToken cancellationToken = default)
        {

            var paramDict = new Dictionary<string, string>();

            if (name is object)
            {
                paramDict["Name"] = name;
            }

            if (postcode is object)
            {
                paramDict["PostCode"] = postcode;
            }

            if (lastChangeDate is object)
            {
                paramDict["LastChangeDate"] = lastChangeDate?.ToString("yyyy-MM-dd");
            }

            if (status is object)
            {
                paramDict["Status"] = status?.ToString();
            }

            if (primaryRoleId is object)
            {
                paramDict["PrimaryRoleId"] = primaryRoleId;
            }

            if (nonPrimaryRoleId is object)
            {
                paramDict["MonPrimaryRoleId"] = primaryRoleId;
            }

            if (roles is object)
            {
                paramDict["Roles"] = string.Join(",", roles.Where(r => r is object));
            }

            if (recordClass is object)
            {
                paramDict["OrgRecordClass"] = recordClass;
            }

            if (!paramDict.Any())
            {
                throw new ArgumentException("At least one search parameter must be given");
            }

            var result = await _httpClient.GetAsync(
                requestUri: CreateSearchUri(paramDict),
                cancellationToken: cancellationToken
            );

            await CheckErrors(result);

            var finalResults = new List<OrganisationSummary>();

            var totalResults = int.Parse(result.Headers.Single(h => h.Key.ToLowerInvariant() == "x-total-count").Value.Single());
            using (var resStream = await result.Content.ReadAsStreamAsync())
            {
                var typedResult = await JsonSerializer.DeserializeAsync<SearchResponse>(resStream, cancellationToken: cancellationToken);
                finalResults.AddRange(typedResult.Organisations);
            }

            if (totalResults > PageSize)
            {
                for (var o = PageSize; o <= totalResults; o += PageSize)
                {
                    finalResults.AddRange(await MakeSearchRequest(
                        paramDict: paramDict,
                        offset: o,
                        cancellationToken: cancellationToken
                        ));
                }
            }

            return finalResults;
        }

        private async Task<IEnumerable<OrganisationSummary>> MakeSearchRequest(IDictionary<string, string> paramDict, int offset, CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.GetAsync(
                requestUri: CreateSearchUri(paramDict, offset),
                cancellationToken: cancellationToken
            );

            using (var resStream = await result.Content.ReadAsStreamAsync())
            {
                var typedResult = await JsonSerializer.DeserializeAsync<SearchResponse>(resStream, cancellationToken: cancellationToken);
                return typedResult.Organisations;
            }
        }

        private string CreateSearchUri(IDictionary<string, string> paramDict, int offset = 0)
        {
            var baseUri = $"{BaseAddress}/organisations?Limit={PageSize}&";
            return baseUri +
                (offset > 0 ? $"Offset={offset}&" : "") +
                string.Join("&", paramDict.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
        }

        private async static Task CheckErrors(HttpResponseMessage result)
        {
            if (!result.IsSuccessStatusCode)
            {
                using (var resStream = await result.Content.ReadAsStreamAsync())
                {
                    var errorResult = await JsonSerializer.DeserializeAsync<ErrorResponse>(resStream, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    throw new APIException(result.StatusCode, errorResult);
                }
            }
        }
    }
}

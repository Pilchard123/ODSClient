using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pilchard123.ODSAPI.APIResponses;

namespace Pilchard123.ODSAPI.APIResults
{
    public class SearchResult
    {
        public int PageSize { get; }
        public int TotalRecords { get; }
        public int TotalPages { get; }
        public int FetchedPages { get; private set; }
        public Criteria Criteria { get; }
        public bool IsComplete => FetchedPages == TotalPages;

        private readonly List<OrganisationSummary> _results;
        private readonly Task _fetchTask;
        private readonly ODSClient _client;
        private readonly CancellationToken _cancellationToken;

        internal SearchResult(Criteria criteria, int pageSize, int totalRecords, IEnumerable<OrganisationSummary> firstPage, ODSClient client,
                              CancellationToken cancellationToken = default)
        {
            Criteria = criteria;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int)Math.Ceiling(totalRecords / (float)pageSize);
            FetchedPages = 1;

            _results = firstPage.ToList();
            _cancellationToken = cancellationToken;
            _client = client;
            _fetchTask = FetchLaterPages();
        }

        public async Task<IEnumerable<OrganisationSummary>> GetResultsAsync()
        {
            await _fetchTask;
            return _results;
        }

        private async Task FetchLaterPages()
        {
            if (TotalRecords > PageSize)
            {
                for (var o = PageSize; o <= TotalRecords; o += PageSize)
                {
                    _cancellationToken.ThrowIfCancellationRequested();

                    _results.AddRange(await _client.GetSearchPageAsync(
                        criteria: Criteria,
                        offset: o,
                        cancellationToken: _cancellationToken
                    ));
                    FetchedPages += 1;
                }
            }
        }

    }

    public class Criteria
    {
        public string Name { get; }
        public string Postcode { get; }
        public DateTime? LastChangeDate { get; }
        public OrganisationStatus? OrganisationStatus { get; }
        public string PrimaryRoleId { get; }
        public string NonPrimaryRoleId { get; }
        public IEnumerable<string> Roles { get; }
        public string RecordClass { get; }

        public ReadOnlyDictionary<string, string> RequestParameters { get; }

        public Criteria(string name = null, string postcode = null, DateTime? lastChangeDate = null, OrganisationStatus? organisationStatus = null,
                        string primaryRoleId = null, string nonPrimaryRoleId = null, IEnumerable<string> roles = null, string recordClass = null)
        {

            var paramDict = new Dictionary<string, string>();

            if (name is object)
            {
                paramDict["Name"] = name;
                Name = name;
            }

            if (postcode is object)
            {
                paramDict["PostCode"] = postcode;
                Postcode = postcode;
            }

            if (lastChangeDate is object)
            {
                paramDict["LastChangeDate"] = lastChangeDate?.ToString("yyyy-MM-dd");
                LastChangeDate = LastChangeDate;
            }

            if (organisationStatus is object)
            {
                paramDict["Status"] = organisationStatus?.ToString();
                OrganisationStatus = organisationStatus;
            }

            if (primaryRoleId is object)
            {
                paramDict["PrimaryRoleId"] = primaryRoleId;
                PrimaryRoleId = primaryRoleId;
            }

            if (nonPrimaryRoleId is object)
            {
                paramDict["NonPrimaryRoleId"] = primaryRoleId;
            }

            if (roles is object)
            {
                roles = roles.Where(r => r is object).ToList();
                paramDict["Roles"] = string.Join(",", roles);
                Roles = roles;
            }

            if (recordClass is object)
            {
                paramDict["OrgRecordClass"] = recordClass;
                RecordClass = recordClass;
            }

            if (!paramDict.Any())
            {
                throw new ArgumentException("At least one search parameter must be given");
            }
            RequestParameters = new ReadOnlyDictionary<string, string>(paramDict);
        }
    }

    public enum OrganisationStatus
    {
        Active,
        Inactive
    }
}

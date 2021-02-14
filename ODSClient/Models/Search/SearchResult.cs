using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pilchard123.ODSAPI.APIResponses;

namespace Pilchard123.ODSAPI.Models.Search
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
}

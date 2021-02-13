namespace Pilchard123.ODSAPI.APIResponses
{

    public class SearchResponse
    {
        public OrganisationSummary[] Organisations { get; set; }
    }

    public class OrganisationSummary
    {
        public string Name { get; set; }
        public string OrgId { get; set; }
        public string Status { get; set; }
        public string OrgRecordClass { get; set; }
        public string PostCode { get; set; }
        public string LastChangeDate { get; set; }
        public string PrimaryRoleId { get; set; }
        public string PrimaryRoleDescription { get; set; }
        public string OrgLink { get; set; }
    }

}

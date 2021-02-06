namespace Pilchard123.ODSAPI.APIResponses
{

    class SyncroniseResponse
    {
        public SyncroniseOrganisation[] Organisations { get; set; }
    }

    class SyncroniseOrganisation
    {
        public string OrgLink { get; set; }
    }

}

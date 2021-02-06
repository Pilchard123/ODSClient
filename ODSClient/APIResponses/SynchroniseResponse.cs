namespace Pilchard123.ODSAPI.APIResponses
{

    class SynchroniseResponse
    {
        public SyncroniseOrganisation[] Organisations { get; set; }
    }

    class SyncroniseOrganisation
    {
        public string OrgLink { get; set; }
    }

}

namespace Pilchard123.ODSAPI.APIResponses
{

    public class RelationshipResponse
    {
        public Relationship[] Relationships { get; set; }
    }

    public class Relationship
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string code { get; set; }
        public string displayName { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

}

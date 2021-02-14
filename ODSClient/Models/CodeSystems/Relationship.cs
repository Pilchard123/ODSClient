namespace Pilchard123.ODSAPI.Models.CodeSystems
{
    public class Relationship
    {
        public static class Identifier
        {
            // RE1 not defined
            public const string IsASubdivisionOf = "RE2";
            public const string IsDirectedBy = "RE3";
            public const string IsComissionedBy = "RE4";
            public const string IsLocateInTheGeographyOf = "RE5";
            public const string IsOperatedBy = "RE6";
            // RE7 not defined
            public const string IsPartnerTo = "RE8";
            public const string IsNominatedPayeeFor = "RE9";
        }
        public string Id { get; }
        public int Code { get; }
        public string DisplayName { get; }

        internal Relationship(string id, int code, string displayName)
        {
            Id = id;
            Code = code;
            DisplayName = displayName;
        }
    }
}

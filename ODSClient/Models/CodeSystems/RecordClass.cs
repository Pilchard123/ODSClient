namespace Pilchard123.ODSAPI.Models.CodeSystems
{
    public class RecordClass
    {
        public static class Identifier
        {
            public const string HSCOrg = "RC1";
            public const string HSCSite = "RC2";
        }

        public string Id { get; }
        public int Code { get; }
        public string DisplayName { get; }

        internal RecordClass(string id, int code, string displayName)
        {
            Id = id;
            Code = code;
            DisplayName = displayName;
        }
    }
}

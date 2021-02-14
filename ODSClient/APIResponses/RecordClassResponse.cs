namespace Pilchard123.ODSAPI.APIResponses
{

    class RecordClassResponse
    {
        public RecordClass[] RecordClasses { get; set; }
    }

    class RecordClass
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string code { get; set; }
        public string displayName { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

}

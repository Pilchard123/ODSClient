namespace Pilchard123.ODSAPI.APIResponses
{

    public class RoleResponse
    {
        public Role[] Roles { get; set; }
    }

    public class Role
    {
#pragma warning disable IDE1006 // Naming Styles
        public string id { get; set; }
        public string code { get; set; }
        public string displayName { get; set; }
        public string primaryRole { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

}

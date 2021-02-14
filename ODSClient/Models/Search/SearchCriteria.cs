using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pilchard123.ODSAPI.Models.Search
{
    public class SearchCriteria
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

        public SearchCriteria(string name = null, string postcode = null, DateTime? lastChangeDate = null, OrganisationStatus? organisationStatus = null,
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
}

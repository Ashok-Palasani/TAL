using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class RolesModel
    {
        public tblrole Role { get; set; }

        public IEnumerable<tblrole> RoleList { get; set; }
    }
}
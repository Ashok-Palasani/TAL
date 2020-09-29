using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class UserModel
    {
        public tbluser Users { get; set; }

        public IEnumerable<tbluser> UsersList { get; set; }
    }
}
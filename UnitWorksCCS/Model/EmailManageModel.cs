using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class EmailManageModel
    {
        public tblmailid Email { get; set; }

        public IEnumerable<tblmailid> EmailList { get; set; }
    }
}
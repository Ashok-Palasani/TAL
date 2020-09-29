using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class MachineModel
    {
        public tblProgramTransferDetailsMaster PT { get; set; }
        public IEnumerable<tblProgramTransferDetailsMaster> PTList { get; set; }
    }
}
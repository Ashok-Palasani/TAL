using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class CellsModel
    {
        public tblcell Cells { get; set; }
        public IEnumerable<tblcell> cellslist { get; set; }

    }
}
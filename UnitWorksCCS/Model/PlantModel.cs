using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class PlantModel
    {
        public tblplant Plant { get; set; }

        public IEnumerable<tblplant> PlantList { get; set; }
    }
}
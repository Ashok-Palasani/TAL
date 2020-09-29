using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitWorksCCS.Models
{
    public class ShopModel
    {

        public tblshop Shops { get; set; }
        public IEnumerable<tblshop> Shopslist {get;set;}
    }
}
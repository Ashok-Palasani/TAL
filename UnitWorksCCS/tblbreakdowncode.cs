//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UnitWorksCCS
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblbreakdowncode
    {
        public int BreakDownCodeID { get; set; }
        public string BreakDownCode { get; set; }
        public string BreakDownCodeDesc { get; set; }
        public string MessageType { get; set; }
        public int BreakDownCodesLevel { get; set; }
        public Nullable<int> BreakDownCodesLevel1ID { get; set; }
        public Nullable<int> BreakDownCodesLevel2ID { get; set; }
        public int IsDeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string ContributeTo { get; set; }
    }
}

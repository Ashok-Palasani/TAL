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
    
    public partial class tblProgramTransferDetailsMaster
    {
        public int PTdMID { get; set; }
        public Nullable<int> PlantID { get; set; }
        public Nullable<int> ProgramType { get; set; }
        public string IpAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<int> Port { get; set; }
        public string Domain { get; set; }
        public int Isdeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public string MachineProgramPath { get; set; }
        public string MachineInvNo { get; set; }
        public string MachineModel { get; set; }
        public string ControllerType { get; set; }
        public string MachineDispName { get; set; }
        public string MachineMake { get; set; }
        public Nullable<int> Shopid { get; set; }
        public Nullable<int> CellId { get; set; }
    
        public virtual tblcell tblcell { get; set; }
        public virtual tblplant tblplant { get; set; }
        public virtual tblprogramType tblprogramType { get; set; }
        public virtual tblshop tblshop { get; set; }
    }
}

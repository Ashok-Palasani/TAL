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
    
    public partial class tblprogramType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblprogramType()
        {
            this.tblProgramTransferDetailsMasters = new HashSet<tblProgramTransferDetailsMaster>();
        }
    
        public int ptypeid { get; set; }
        public string TypeName { get; set; }
        public int Isdeleted { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblProgramTransferDetailsMaster> tblProgramTransferDetailsMasters { get; set; }
    }
}

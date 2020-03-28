namespace NorthwindEF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CreditCard")]
    public partial class CreditCard
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CreditCardId { get; set; }

        [Required]
        [StringLength(50)]
        public string CardHolder { get; set; }

        [Column(TypeName = "date")]
        public DateTime ExpirationDate { get; set; }

        public int EmployeeID { get; set; }

        public virtual Employee Employee { get; set; }
    }
}

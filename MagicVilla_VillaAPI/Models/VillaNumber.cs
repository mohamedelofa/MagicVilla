namespace MagicVilla_VillaAPI.Models
{
    public class VillaNumber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VillNo { get; set; }
        public string SpecialDetails { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [ForeignKey(nameof(Villa))]
        public int VillaId { get; set; }
        public Villa? Villa { get; set; } 
    }
}

namespace MagicVilla_VillaAPI.Models.Dtos
{
    public class GetVillaDto : BaseVillaDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
    }
}

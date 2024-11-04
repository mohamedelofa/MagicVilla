namespace MagicVilla_VillaAPI.Profiles
{
    public class VillaNumberProfile : Profile
    {
        public VillaNumberProfile()
        {
            CreateMap<VillaNumber, GetVillaNumberDto>();
            CreateMap<CreateVillaNumberDto, VillaNumber>().ForMember(
                    dest => dest.CreatedDate,
                    src => src.MapFrom(v => DateTime.Now)
                );
            CreateMap<UpdateVillaNumberDto, VillaNumber>().ForMember(
                    dest => dest.UpdatedDate,
                    src => src.MapFrom(v => DateTime.Now)
                );
        }
    }
}

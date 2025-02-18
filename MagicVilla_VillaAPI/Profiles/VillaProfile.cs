namespace MagicVilla_VillaAPI.Profiles
{
    public class VillaProfile : Profile
    {
        public VillaProfile()
        {
            CreateMap<Villa, GetVillaDto>()
                .ForMember(dest => dest.ImageUrl,
                           src => src.MapFrom(v => v.ImageUrl));
            CreateMap<CreateVillaDto, Villa>()
                .ForMember(dest => dest.CreatedDate,
                           src => src.MapFrom(v => DateTime.Now));
            CreateMap<UpdateVillaDto, Villa>()
                .ForMember(dest => dest.UpdatedDate,
                           src => src.MapFrom(v => DateTime.Now))
                .ForMember(dest => dest.ImageName,
                           src => src.Ignore());

            CreateMap<CreateUpdateVillaDto, Villa>()
                .ForMember(
                    dest => dest.CreatedDate,
                    src => src.MapFrom(x => DateTime.Now)
                );
        }
    }
}

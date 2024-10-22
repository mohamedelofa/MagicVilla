namespace MagicVilla_VillaAPI.Profiles
{
    public class VillaProfile :Profile
    {
        public VillaProfile()
        {
            CreateMap<Villa, GetVillaDto>();

            CreateMap<CreateUpdateVillaDto, Villa>()
                .ForMember(
                    dest => dest.CreatedDate,
                    src => src.MapFrom(x => DateTime.Now)
                );
        }
    }
}

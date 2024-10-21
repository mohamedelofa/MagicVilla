namespace MagicVilla_VillaAPI.Profiles
{
    public class UserProfile :Profile
    {
        public UserProfile()
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

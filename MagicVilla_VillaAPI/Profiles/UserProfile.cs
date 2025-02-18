namespace MagicVilla_VillaAPI.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterRequestDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}

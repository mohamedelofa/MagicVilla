namespace MagicVilla_VillaAPI.Profiles
{
	public class UserProfile : Profile
	{
        public UserProfile()
        {
            CreateMap<RegisterRequestDto, LocalUser>();
        }
    }
}

namespace MagicVilla_VillaAPI.Profiles
{
	public class UserProfile : Profile
	{
        public UserProfile()
        {
            CreateMap<RegisterRequestDto, ApplicationUser>()
                .ForMember(dest => dest.PasswordHash,
                src => src.MapFrom(x => x.Password));
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}

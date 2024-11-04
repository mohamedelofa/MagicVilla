using AutoMapper;
using MagicVilla_WebApp.Models.Dtos;
using MagicVilla_WebApp.Models.ViewModels;

namespace MagicVilla_WebApp.Profiles
{
    public class MappingConfig :Profile
    {
        public MappingConfig()
        {
            CreateMap<GetVillaDto,UpdateVillaDto>().ReverseMap();
            CreateMap<CreateVillaNumberViewModel, CreateVillaNumberDto>().ReverseMap();
            CreateMap<GetVillaNumberDto, UpdateVillaNumberViewModel>().ReverseMap();
            CreateMap<UpdateVillaNumberViewModel, UpdateVillaNumberDto>().ReverseMap();

        }
    }
}

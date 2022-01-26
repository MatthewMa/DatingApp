using API.DTOS;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Tools
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDTO>().ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDTO>();
            CreateMap<MemberUpdateDTO, AppUser>();
            CreateMap<AccountRegisterDTO, AppUser>();
            CreateMap<AppUser, LikeDTO>().ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Message, MessageDTO>().ForMember(dest => dest.SenderPhotoUrl,
                opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.ReceipientPhotoUrl,
                opt => opt.MapFrom(src => src.Receipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}

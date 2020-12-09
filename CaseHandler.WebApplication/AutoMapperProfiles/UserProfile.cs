using AutoMapper;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Models.ViewModels;

namespace CaseHandler.WebApplication.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, ListUsersViewModel>()
                .ForMember(dest => dest.IsLockedOut, opt => opt.MapFrom(src => src.LockoutEnd == null ? false : true))
                .ForMember(dest => dest.CaseCount, opt => opt.MapFrom(src => src.ReportedCases.Count))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.Comments.Count));
            CreateMap<ApplicationUser, EditUserRequestModel>()
                .ForMember(dest => dest.IsLockedOut, opt => opt.MapFrom(src => src.LockoutEnd == null ? false : true));
        }
    }
}

using AutoMapper;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.ViewModels;

namespace CaseHandler.WebApplication.AutoMapperProfiles
{
    public class HistoryProfile : Profile
    {
        public HistoryProfile()
        {
            CreateMap<History, HistoryItemViewModel>()
                .ForMember(dst => dst.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedBy.UserName));
        }
    }
}

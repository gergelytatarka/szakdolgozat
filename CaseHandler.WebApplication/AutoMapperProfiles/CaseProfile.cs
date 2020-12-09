using AutoMapper;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Models.ViewModels;
using System;

namespace CaseHandler.WebApplication.AutoMapperProfiles
{
    public class CaseProfile : Profile
    {
        public CaseProfile()
        {
            CreateMap<CreateCaseRequestModel, Case>();
            CreateMap<EditCaseRequestModel, Case>();
            CreateMap<Case, EditCaseRequestModel>();
            CreateMap<Case, CaseDetailsViewModel>();
            CreateMap<Case, CaseItemViewModel>()
                .ForMember(dst => dst.OpenDaysCount, opt => opt.MapFrom(src => (int)(DateTime.Now - src.ReportedAt).TotalDays))
                .ForMember(dst => dst.DaysUntilDeadline, opt => opt.MapFrom(src => (int)(src.Deadline.GetValueOrDefault() - DateTime.Now).TotalDays));
        }
    }
}

using AutoMapper;
using CaseHandler.WebApplication.Data.Models;
using CaseHandler.WebApplication.Models.RequestModels;
using CaseHandler.WebApplication.Models.ViewModels;

namespace CaseHandler.WebApplication.AutoMapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentListItemViewModel>();
            CreateMap<Comment, EditCommentRequestModel>().ReverseMap();
        }
    }
}

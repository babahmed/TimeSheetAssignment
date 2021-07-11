
using HimamaTimesheet.Web.Areas.Catalog.Models;
using AutoMapper;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetAll;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetById;
using HimamaTimesheet.Application.Features.Tracker.Commands.Create;

namespace HimamaTimesheet.Web.Areas.Catalog.Mappings
{
    internal class TrackerProfile : Profile
    {
        public TrackerProfile()
        {
            CreateMap<GetAllTrackerResponse, TrackerViewModel>().ReverseMap();
            CreateMap<GetTrackerByIdResponse, TrackerViewModel>().ReverseMap();
            CreateMap<CreateTrackerCommand, TrackerViewModel>().ReverseMap();
            CreateMap<UpdateTrackerCommand, TrackerViewModel>().ReverseMap();
        }
    }
}
using HimamaTimesheet.Domain.Entities.Catalog;
using AutoMapper;
using HimamaTimesheet.Application.Features.Tracker.Commands.Create;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetAll;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetById;

namespace HimamaTimesheet.Application.Mappings
{
    internal class TimeSheetProfile : Profile
    {
        public TimeSheetProfile()
        {
            CreateMap<CreateTrackerCommand, Timesheet>().ReverseMap();
            CreateMap<GetAllTrackerResponse, Timesheet>().ReverseMap();
            CreateMap<GetTrackerByIdResponse, Timesheet>().ReverseMap();
        }
    }
}
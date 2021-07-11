
using AspNetCoreHero.Results;
using AutoMapper;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetById;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HimamaTimesheet.Application.Features.Tracker.Queries.GetOpen
{
    public class GetOpenTrackerQuery : IRequest<Result<GetTrackerByIdResponse>>
    {
        public GetOpenTrackerQuery(string userId)
        {
            UserId = userId;
        }
        public string UserId { get; set; }

        public class GetOpenTrackerQueryHandler : IRequestHandler<GetOpenTrackerQuery, Result<GetTrackerByIdResponse>>
        {
            private readonly IGenericRepository<Timesheet> _timeSheets;
            private readonly IMapper _mapper;

            public GetOpenTrackerQueryHandler(IGenericRepository<Timesheet> timeSheets, IMapper mapper)
            {
                _timeSheets = timeSheets;
                _mapper = mapper;
            }

            public async Task<Result<GetTrackerByIdResponse>> Handle(GetOpenTrackerQuery query, CancellationToken cancellationToken)
            {
                var trackSheet = await _timeSheets.GetAsync(c=>c.UserId==query.UserId && c.TimeOut == null);

                if (trackSheet == null)
                {
                    return Result<GetTrackerByIdResponse>.Success(null,"User have no pending clock out");
                }
                var mappedData = _mapper.Map<GetTrackerByIdResponse>(trackSheet);

                return Result<GetTrackerByIdResponse>.Success(mappedData,"You have a clock out pending");
            }
        }
    }
}
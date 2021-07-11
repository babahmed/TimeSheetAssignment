
using AspNetCoreHero.Results;
using AutoMapper;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HimamaTimesheet.Application.Features.Tracker.Queries.GetAll
{
    public class GetAllTrackerQuery : IRequest<Result<List<GetAllTrackerResponse>>>
    {
        public string UserId { get; set; }
        public GetAllTrackerQuery(string userId)
        {
            UserId = userId;
        }
    }

    public class GetAllTrackerQueryHandler : IRequestHandler<GetAllTrackerQuery, Result<List<GetAllTrackerResponse>>>
    {
        private readonly IGenericRepository<Timesheet> _timeSheets;
        private readonly IMapper _mapper;

        public GetAllTrackerQueryHandler(IGenericRepository<Timesheet> timeSheets, IMapper mapper)
        {
            _timeSheets = timeSheets;
            _mapper = mapper;
        }

        public async Task<Result<List<GetAllTrackerResponse>>> Handle(GetAllTrackerQuery request, CancellationToken cancellationToken)
        {
            var data = await _timeSheets.GetAllAsync(c=>c.UserId==request.UserId);
            var mappeddata = _mapper.Map<List<GetAllTrackerResponse>>(data.OrderByDescending(c=>c.CreatedOn));
            return Result<List<GetAllTrackerResponse>>.Success(mappeddata);
        }
    }
}
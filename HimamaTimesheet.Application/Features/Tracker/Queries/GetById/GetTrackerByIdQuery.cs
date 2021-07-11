
using AspNetCoreHero.Results;
using AutoMapper;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace HimamaTimesheet.Application.Features.Tracker.Queries.GetById
{
    public class GetTrackerByIdQuery : IRequest<Result<GetTrackerByIdResponse>>
    {
        public GetTrackerByIdQuery(int id)
        {
            Id = id;
        }
        public int Id { get; set; }

        public class GetTrackerByIdQueryHandler : IRequestHandler<GetTrackerByIdQuery, Result<GetTrackerByIdResponse>>
        {
            private readonly IGenericRepository<Timesheet> _timeSheets;
            private readonly IMapper _mapper;

            public GetTrackerByIdQueryHandler(IGenericRepository<Timesheet> timeSheets, IMapper mapper)
            {
                _timeSheets = timeSheets;
                _mapper = mapper;
            }

            public async Task<Result<GetTrackerByIdResponse>> Handle(GetTrackerByIdQuery query, CancellationToken cancellationToken)
            {
                var trackSheet = await _timeSheets.GetByIdAsync(query.Id);
                var mappedData = _mapper.Map<GetTrackerByIdResponse>(trackSheet);
                return Result<GetTrackerByIdResponse>.Success(mappedData);
            }
        }
    }
}
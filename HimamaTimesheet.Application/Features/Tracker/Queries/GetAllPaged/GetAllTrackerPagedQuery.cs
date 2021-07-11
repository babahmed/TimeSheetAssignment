using HimamaTimesheet.Application.Extensions;
using AspNetCoreHero.Results;
using MediatR;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetAll;
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;

namespace HimamaTimesheet.Application.Features.Tracker.Queries.GetAllPaged
{
    public class GetAllTrackerPagedQuery : IRequest<PaginatedResult<GetAllTrackerResponse>>
    {
        public string UserId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public GetAllTrackerPagedQuery(int pageNumber, int pageSize, string userId)
        {
            UserId = userId;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetAllTrackerPagedQueryHandler : IRequestHandler<GetAllTrackerPagedQuery, PaginatedResult<GetAllTrackerResponse>>
    {
        private readonly IGenericRepository<Timesheet> _timeSheet;

        public GetAllTrackerPagedQueryHandler(IGenericRepository<Timesheet> timeSheet)
        {
            _timeSheet = timeSheet;
        }

        public async Task<PaginatedResult<GetAllTrackerResponse>> Handle(GetAllTrackerPagedQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Timesheet, GetAllTrackerResponse>> expression = e => new GetAllTrackerResponse
            {
                Id = e.Id,
                TimeIn = e.TimeIn,
                TimeOut = e.TimeOut,
                UserId = e.UserId,
            };
            var data = await _timeSheet.GetAllAsync(c=>c.UserId!=null);

            var paginatedList = await data.Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return paginatedList;
        }
    }
}
using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetOpen;
using FluentValidation;

namespace HimamaTimesheet.Application.Features.Tracker.Commands.Create
{
    public partial class CreateTrackerCommand : IRequest<Result<int>>
    {
        public string UserId { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
    }
    public class CreateTrackerCommandValidator : AbstractValidator<CreateTrackerCommand>
    {
        public CreateTrackerCommandValidator()
        {
            RuleFor(c => c.UserId).NotNull();

        }
    }

    public class CreateTrackerCommandHandler : IRequestHandler<CreateTrackerCommand, Result<int>>
    {
        private readonly IGenericRepository<Timesheet> _timeSheet;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        private IUnitOfWork _unitOfWork { get; set; }

        public CreateTrackerCommandHandler(IGenericRepository<Timesheet> timeSheet, IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _timeSheet = timeSheet;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<int>> Handle(CreateTrackerCommand request, CancellationToken cancellationToken)
        {
            if (request.TimeOut.HasValue && request.TimeOut.Value == DateTime.MinValue) request.TimeOut = null;
            var existingTracker = await _mediator.Send(new GetOpenTrackerQuery(request.UserId));

            if (existingTracker == null)
            {
                return Result<int>.Fail(existingTracker.Message ?? "Something went wrong!!");
            }

            //check existing checking
            if (existingTracker?.Data != null)
            {
                return Result<int>.Fail("User have pending clock out, kindly complete clock out before clocking back in");
            }

            var overlapingTracker = await _timeSheet.GetAsync(c => c.TimeIn > request.TimeIn && c.TimeOut < request.TimeIn);
            //Check overlapping checking
            if (overlapingTracker != null)
            {
                return Result<int>.Fail("You clock in period is overlapping one of your Time sheet");
            }

            var tracker = _mapper.Map<Timesheet>(request);
            await _timeSheet.AddAsync(tracker);
            await _unitOfWork.Commit(cancellationToken);
            return Result<int>.Success(tracker.Id, "clocked in successfully");
        }
    }
}
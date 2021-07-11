using HimamaTimesheet.Application.Interfaces.Repositories;
using HimamaTimesheet.Domain.Entities.Catalog;
using AspNetCoreHero.Results;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System;
using FluentValidation;

namespace HimamaTimesheet.Application.Features.Tracker.Commands.Create
{
    public class UpdateTrackerCommand : IRequest<Result<long>>
    {
        public int Id { get; set; }
        public DateTime? TimeOut { get; set; }
        public DateTime? TimeIn { get; set; }

        public class UpdateTrackerCommandValidator : AbstractValidator<UpdateTrackerCommand>
        {
            public UpdateTrackerCommandValidator()
            {
                RuleFor(c => c.Id).NotEqual(0);

            }
        }

        public class UpdateTrackerCommandHandler : IRequestHandler<UpdateTrackerCommand, Result<long>>
        {
            private readonly IGenericRepository<Timesheet> _timeSheet;

            private IUnitOfWork _unitOfWork { get; set; }

            public UpdateTrackerCommandHandler(IGenericRepository<Timesheet> timeSheet, IUnitOfWork unitOfWork)
            {
                _timeSheet = timeSheet;
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<long>> Handle(UpdateTrackerCommand command, CancellationToken cancellationToken)
            {
                if (command.TimeOut.HasValue && command.TimeOut.Value == DateTime.MinValue) command.TimeOut = null;

                var TrackSheet = await _timeSheet.GetByIdAsync(command.Id);

                if (TrackSheet == null)
                {
                    return Result<long>.Fail($"TimeSheet Not Found.");
                }
                if (TrackSheet.TimeOut != null)
                {
                    return Result<long>.Fail($"Tracker already completed., Please clock in");
                }
                else
                {

                    if (command.TimeIn != null && await isOverlappingAsync(command.Id, command.TimeIn.Value)
                        || command.TimeOut != null && await isOverlappingAsync(command.Id, command.TimeOut.Value))
                    {
                        return Result<long>.Fail($"Modification will overlapp existing timesheet");
                    }
                    TrackSheet.TimeOut = command.TimeOut ?? TrackSheet.TimeOut;
                    TrackSheet.TimeIn = command.TimeIn ?? TrackSheet.TimeIn;

                    await _timeSheet.UpdateAsync(TrackSheet);
                    await _unitOfWork.Commit(cancellationToken);

                    return Result<long>.Success(TrackSheet.Id, "Time Sheet updated successfully");
                }
            }

            private async Task<bool> isOverlappingAsync(int Id, DateTime time)
            {
                //check if this will overlap existing timesheet
                return await _timeSheet.GetAsync(x => x.Id != Id && x.TimeIn > time && x.TimeOut < time) != null;
            }
        }
    }
}
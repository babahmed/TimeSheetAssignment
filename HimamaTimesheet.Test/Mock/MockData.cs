using Bogus;
using HimamaTimesheet.Application.Features.Tracker.Commands.Create;
using System;
namespace HimamaTimesheet.Test.Mock
{
    public static class Mock
    {

        public static Faker<CreateTrackerCommand> CreateTrackerFaker(string UserId)
        {
            Faker<CreateTrackerCommand> fakerTracker = new Faker<CreateTrackerCommand>()
                .RuleFor(o => o.UserId, f => UserId)
                .RuleFor(o => o.TimeIn, f=> DateTime.Now.AddMinutes(-10))
                .RuleFor(o => o.TimeOut, f=> DateTime.Now);

            return fakerTracker;
        }

        public static Faker<UpdateTrackerCommand> UpdateTrackerFaker(int id)
        {
            Faker<UpdateTrackerCommand> fakerUpdate = new Faker<UpdateTrackerCommand>()
                .RuleFor(o => o.Id, id)
                .RuleFor(o => o.TimeIn, f => DateTime.Now.AddMinutes(-10))
                .RuleFor(o => o.TimeOut, f => DateTime.Now);

            return fakerUpdate;
        }

    }
}

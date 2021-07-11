using HimamaTimesheet.Application.Interfaces.Shared;
using System;

namespace HimamaTimesheet.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}
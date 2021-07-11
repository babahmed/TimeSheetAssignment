using System;

namespace HimamaTimesheet.Application.Features.Tracker.Queries.GetById
{
    public class GetTrackerByIdResponse
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
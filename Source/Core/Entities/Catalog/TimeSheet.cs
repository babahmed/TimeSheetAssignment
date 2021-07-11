using AspNetCoreHero.Abstractions.Domain;
using System;

namespace HimamaTimesheet.Domain.Entities.Catalog
{
    public class Timesheet : AuditableEntity
    {
        public string UserId { get; set; }
        public DateTime TimeIn { get; set; } = DateTime.Now;
        public DateTime? TimeOut { get; set; }
    }
}
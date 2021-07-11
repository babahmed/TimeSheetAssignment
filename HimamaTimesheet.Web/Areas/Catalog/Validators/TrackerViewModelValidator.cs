using HimamaTimesheet.Web.Areas.Catalog.Models;
using FluentValidation;
using System;

namespace HimamaTimesheet.Web.Areas.Catalog.Validators
{
    public class TrackerViewModelValidator : AbstractValidator<TrackerViewModel>
    {
        public TrackerViewModelValidator()
        {
            RuleFor(p => p.TimeIn)
                .NotEmpty()
                .NotEqual(DateTime.MinValue)
                .NotNull().WithMessage("{PropertyName} valid value is required.");

            RuleFor(p => p.TimeIn)
                .NotEmpty().GreaterThan(p=>p.TimeIn)
                .NotNull().WithMessage("{PropertyName} valid value is required.");
        }
    }
}
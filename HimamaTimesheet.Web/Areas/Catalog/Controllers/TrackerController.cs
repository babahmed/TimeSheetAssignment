
using HimamaTimesheet.Application.Features.Tracker.Commands.Create;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetAll;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetById;
using HimamaTimesheet.Application.Features.Tracker.Queries.GetOpen;
using HimamaTimesheet.Web.Abstractions;
using HimamaTimesheet.Web.Areas.Catalog.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HimamaTimesheet.Web.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    public class TrackerController : BaseController<TrackerController>
    {
        public async Task<IActionResult> Index()
        {
            var model = new TrackerViewModel() { TimeIn = DateTime.Now, TimeOut = DateTime.MinValue };
            var pendingTracker = await _mediator.Send(new GetOpenTrackerQuery(_user.UserId));
           
            if (pendingTracker.Succeeded)
            {
                model = _mapper.Map<TrackerViewModel>(pendingTracker.Data);
            }
            
            return View(model);
        }

        public async Task<IActionResult> LoadAll()
        {
            var response = await _mediator.Send(new GetAllTrackerQuery(_user.UserId));
            if (response.Succeeded)
            {
                var viewModel = _mapper.Map<List<TrackerViewModel>>(response.Data);
                return PartialView("_ViewAll", viewModel);
            }
            return null;
        }

        public async Task<JsonResult> OnGetCreateOrEdit(int id)
        {
            if (id == 0)
            {
                var pendingTracker = await _mediator.Send(new GetOpenTrackerQuery(_user.UserId));
                if (pendingTracker == null || pendingTracker.Failed)
                {
                    _notify.Information(pendingTracker?.Message ?? "Unable to validate pending clockin status");
                    return null;
                }
                if (pendingTracker.Data == null)
                {
                    var TrackerViewModel = new TrackerViewModel() { TimeIn = DateTime.Now, TimeOut = DateTime.MinValue };
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", TrackerViewModel) });
                }
                else
                {
                    var TrackerViewModel = _mapper.Map<TrackerViewModel>(pendingTracker.Data);
                    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", TrackerViewModel) });
                }
            }
            else
            {
                var Tracker = await _mediator.Send(new GetTrackerByIdQuery(id));
                var TrackerViewModel = _mapper.Map<TrackerViewModel>(Tracker.Data);
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", TrackerViewModel) });
            }

        }

        [HttpPost]
        public async Task<JsonResult> OnPostCreateOrEdit(int id, TrackerViewModel tracker)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    var createTrackerCommand = _mapper.Map<CreateTrackerCommand>(tracker);
                    createTrackerCommand.UserId = _user.UserId;
                    var result = await _mediator.Send(createTrackerCommand);
                    if (result.Succeeded)
                    {
                        id = result.Data;
                        _notify.Success($"{result.Message}");
                    }
                    else _notify.Error(result.Message);
                }
                else
                {
                    var updateTrackerCommand = _mapper.Map<UpdateTrackerCommand>(tracker);
                    var result = await _mediator.Send(updateTrackerCommand);
                    if (result.Succeeded) _notify.Information($"{result.Message}");
                }

                var response = await _mediator.Send(new GetAllTrackerQuery(_user.UserId));
                if (response.Succeeded)
                {
                    var viewModel = _mapper.Map<List<TrackerViewModel>>(response.Data);
                    var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
                    return new JsonResult(new { isValid = true, html = html });
                }
                else
                {
                    _notify.Error(response.Message);
                    return null;
                }
            }
            else
            {
                var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", tracker);
                return new JsonResult(new { isValid = false, html = html });
            }
        }
    }
}
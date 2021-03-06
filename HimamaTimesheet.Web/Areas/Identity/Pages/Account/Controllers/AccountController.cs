using HimamaTimesheet.Web.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HimamaTimesheet.Application.Features.ActivityLog.Commands.AddLog;
using HimamaTimesheet.Infrastructure.Identity.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Security.Claims;

namespace HimamaTimesheet.Web.Areas.Account.Controllers
{
    [Area("Identity"),Authorize]
    public class AccountController : BaseController<AccountController>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            _notify.Information("Hi There!");
            return View();
        }
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return new ChallengeResult("Google", properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse()
        {
            var returnUrl = Url.Content("~/");
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                return LocalRedirect("/Identity/Account/Login");

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);            
            if (result.Succeeded)
            {
                var usr = await _userManager.FindByNameAsync(info.Principal.FindFirst(ClaimTypes.Email).Value);
                await _mediator.Send(new AddActivityLogCommand() { userId = usr.Id, Action = "Logged In" });
                _logger.LogInformation("User logged in.");
                _notify.Success($"Logged in as {info.Principal.FindFirst(ClaimTypes.Name).Value}.");
                return LocalRedirect(returnUrl);
            }
            else
            {
                ApplicationUser user = new ApplicationUser
                {
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    FirstName = info.Principal.FindFirst(ClaimTypes.GivenName).Value,
                    LastName = info.Principal.FindFirst(ClaimTypes.Surname).Value,
                    EmailConfirmed=true,
                    IsActive = true
                };

                IdentityResult identResult = await _userManager.CreateAsync(user);
                if (identResult.Succeeded)
                {
                    identResult = await _userManager.AddLoginAsync(user, info);
                    if (identResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        //return RedirectToAction("Index","Home");
                        return LocalRedirect(returnUrl);
                    }
                }
                return LocalRedirect("/identity/account/accessDenied");
            }
        }
    }
}
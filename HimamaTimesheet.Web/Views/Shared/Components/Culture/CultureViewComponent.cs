using Microsoft.AspNetCore.Mvc;

namespace HimamaTimesheet.Web.Views.Shared.Components.Culture
{
    public class CultureViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
using System.Threading.Tasks;

namespace HimamaTimesheet.Web.Abstractions
{
    public interface IViewRenderService
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
	
}
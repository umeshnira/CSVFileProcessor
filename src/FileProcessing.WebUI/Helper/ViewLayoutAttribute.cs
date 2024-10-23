using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.WebUI.Helper
{
    public class ViewLayoutAttribute : ResultFilterAttribute
    {
        private string layout;
        public ViewLayoutAttribute(string layout)
        {
            this.layout = layout;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var viewResult = context.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["Layout"] = this.layout;
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shop.Api.Filter
{
    public class FileFormatFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var file = context.HttpContext.Request.Form.Files.FirstOrDefault();
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!validExtensions.Contains(extension.ToLower()))
                {
                    context.Result = new BadRequestObjectResult("Required file format is jpeg, jpg, png, gif !!");
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("No file was uploaded.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BECAS.Filters
{
    public class ResponseHeaderAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string name = context.HttpContext.Session.GetString("SessionKeyName");
            if (string.IsNullOrEmpty(name))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                            {"controller", "Login"},
                            {"action", "Index" }
                    }
                    );

            }
        }
          
    }
}

using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace WeaselServicesAPI.Attributes
{
    public class MobileAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            const string AuthHeader = "X-Mobile-Authorization";
            
            var ctx = context.HttpContext.RequestServices.GetService<ServicesAPIContext>();

            context.HttpContext.Request.Headers.TryGetValue(AuthHeader, out var value);

            if (value.Count == 0)
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "No device identifier was provided!" });
            }

            Guid uuid = Guid.Empty;

            if (Guid.TryParse(value.FirstOrDefault(), out Guid uuidParse)) uuid = uuidParse;

            if (uuid == Guid.Empty)
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Failed to parse device identifier!" });
            }
            else
            {
                var device = ctx.Devices.FirstOrDefault(d => d.Uuid == uuid);

                if (device == null)
                {
                    context.Result = new UnauthorizedObjectResult(new { Message = "Could not find a device with the identifier provided!" });
                }

                // add device for any future reference
                context.HttpContext.Items.Add("Device", device);
            }
        }
    }
}

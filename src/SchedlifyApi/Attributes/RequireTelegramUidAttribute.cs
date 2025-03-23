namespace SchedlifyApi.Attributes;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireTelegramUidAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if the Telegram UID header is present
        if (!context.HttpContext.Request.Headers.TryGetValue("X-TG-UID", out var telegramUidValue))
        {
            context.Result = new UnauthorizedObjectResult("Telegram UID is required");
            return;
        }

        // Parse the Telegram UID to a long (bigint)
        if (!long.TryParse(telegramUidValue, out var telegramUid))
        {
            context.Result = new BadRequestObjectResult("Invalid Telegram UID format. Must be a number.");
            return;
        }

        // Add the parsed Telegram UID to the controller's action arguments
        context.ActionArguments["telegramUid"] = telegramUid;

        // Continue with the action execution
        await next();
    }
}
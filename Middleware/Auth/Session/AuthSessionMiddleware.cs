using AspKnP231.Data.Entities;
using System.Text.Json;
using System.Security.Claims;

namespace AspKnP231.Middleware.Auth.Session
{
    public class AuthSessionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            // детектуємо запит на вихід з авторизованого режиму
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove("UserAccess");
                context.Response.Redirect(context.Request.Path);
                return;   // перериваємо ланцюг Middleware
            }

            if (context.Session.Keys.Contains("UserAccess"))
            {
                // У сесії є інформація про вхід (автентифікацію)
                if(JsonSerializer.Deserialize<UserAccess>(
                    context.Session.GetString("UserAccess")!) is UserAccess userAccess)
                {
                    // context.Items.Add("UserAccess", userAccess); -- неоптимальний
                    // підхід - створює сильну залежність від типів даних з шару БД
                    // Слід перейти на універсальні інтерфейси - System.Security.Claims

                    context.User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.Name, userAccess.UserData.Name),
                            new Claim(ClaimTypes.Email, userAccess.UserData.Email),
                            new Claim(ClaimTypes.NameIdentifier, userAccess.Login),
                            new Claim(ClaimTypes.Thumbprint, userAccess.AvatarFilename ?? ""),
                            new Claim(ClaimTypes.DateOfBirth, userAccess.UserData.Birthdate.ToShortDateString()),
                            new Claim(ClaimTypes.Role, 
                                userAccess.UserRoleId == Guid.Parse("250FA2D3-0818-42D6-A1ED-112F115407D6")
                                ? "Admin"
                                : "Guest"
                            ),
                        ],
                        nameof(AuthSessionMiddleware)
                    ));
                }
            }
            await _next(context);
        }
    }
}

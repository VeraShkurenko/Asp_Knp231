using AspKnP231.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspKnP231.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public UsersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // 1. Перевірка автентифікації (401)
            var authLogin = HttpContext.Session.GetString("auth-login");
            if (string.IsNullOrEmpty(authLogin))
            {
                return Unauthorized(new { message = "Ви не автентифіковані" });
            }

            // 2. Перевірка авторизації (403)
            // У нашому спрощеному випадку адмін - це той, у кого логін "admin"
            // (Або можна перевіряти роль з БД)
            if (authLogin != "admin")
            {
                return StatusCode(403, new { message = "Доступ заборонено: ви не є адміністратором" });
            }

            // 3. Повернення даних
            var users = await _dataContext.UserAccesses
                .Include(ua => ua.UserData)
                .Select(ua => new
                {
                    ua.Login,
                    Name = ua.UserData.Name,
                    Email = ua.UserData.Email,
                    ua.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}

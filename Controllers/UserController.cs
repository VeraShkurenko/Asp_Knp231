using AspKnP231.Data;
using AspKnP231.Data.Entities;
using AspKnP231.Models.Home;
using AspKnP231.Models.User;
using AspKnP231.Services.Kdf;
using AspKnP231.Services.Storage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using System.Text;
using System.Text.Json;

namespace AspKnP231.Controllers
{
    public class UserController(DataContext dataContext, IStorageService storageService, IKdfService kdfService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;
        private readonly IKdfService _kdfService = kdfService;

        public IActionResult Index()
        {
            ViewData["JwtModel"] = new JwtModel
            {
                Payload = new()
                {
                    Name = "User",
                    Email = "user@i.ua",
                    Dob = "2020-02-02",
                }
            };
            return View();
        }

        public IActionResult Profile()
        {
            // Захищаємо сторінку від неавторизованого доступу
            if(HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }
            return Redirect("/");
        }

        public JsonResult TestAuth()
        {
            String authHeader = Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(authHeader))
            {
                return Json(new
                {
                    status = 401,
                    data = "Missing 'Authorization' header"
                });
            }
            String scheme = "Bearer ";
            if (!authHeader.StartsWith(scheme))
            {
                return Json(new
                {
                    status = 401,
                    data = "Invalid 'Authorization' scheme. Must be " + scheme
                });
            }
            String token = authHeader[scheme.Length..];
            // Валідація токена за https://datatracker.ietf.org/doc/html/rfc7519#section-7.2
            int dotPosition = token.IndexOf('.');
            if(dotPosition == -1)
            {
                return Json(new
                {
                    status = 401,
                    data = "The JWT must contain at least one period ('.') character "
                });
            }
            String header = token[..dotPosition];

            // Base64url decode the Encoded JOSE Header following the
            // restriction that no line breaks, whitespace, or other additional
            // characters have been used. Verify that the resulting octet sequence is a UTF-8-encoded...
            String decodedHeader;
            try
            {
                decodedHeader = Encoding.UTF8.GetString( Base64UrlTextEncoder.Decode(header) );
            }
            catch
            {
                return Json(new
                {
                    status = 401,
                    data = "The JWT header decode error "
                });
            }

            // 
            JwtHeader jwtHeader;
            try
            {
                jwtHeader = JsonSerializer.Deserialize<JwtHeader>(decodedHeader, JwtModel.options)!;
            }
            catch
            {
                return Json(new
                {
                    status = 401,
                    data = "The JWT header must carry valid JSON"
                });
            }
            if(jwtHeader.Typ != "JWT")
            {
                return Json(new
                {
                    status = 401,
                    data = "The JWT header.typ unsupported: 'JWT' only"
                });
            }
            if(jwtHeader.Alg != "HS256")
            {
                return Json(new
                {
                    status = 401,
                    data = "The JWT header.alg unsupported: 'HS256' only"
                });
            }

            // Відокремлюємо підпис та підписану частину
            dotPosition = token.LastIndexOf('.');
            String signedPart = token[..dotPosition];
            String signature = token[(dotPosition + 1)..];
            String jwtSignature = JwtModel.Sign64(signedPart);
            if(jwtSignature != signature)
            {
                return Json(new
                {
                    status = 401,
                    data = "Signature error",
                    signature,
                    jwtSignature
                });
            }

            return Json(new
            {
                status = 200,
                data = decodedHeader,
                signedPart,signature
            });
        }

        public IActionResult SignUp()
        {
            UserSignupViewModel viewModel = new();
            if (HttpContext.Session.Keys.Contains(nameof(UserSignupFormModel)))
            {
                // є збережені у сесії дані, тоді відновлюємо, використовуємо та видаляємо
                viewModel.FormModel = JsonSerializer.Deserialize<UserSignupFormModel>(
                    HttpContext.Session.GetString(nameof(UserSignupFormModel))!
                );
                ModelStateDictionary modelState = new();

                JsonElement savedState = JsonSerializer.Deserialize<JsonElement>(
                    HttpContext.Session.GetString(nameof(ModelState))!
                )!;
                foreach (var item in savedState.EnumerateObject())
                {
                    var errors = item.Value.GetProperty("Errors");
                    if (errors.GetArrayLength() > 0)
                    {
                        foreach (var err in errors.EnumerateArray())
                        {
                            modelState.AddModelError(item.Name, err.GetProperty("ErrorMessage").GetString()!);
                        }
                    }
                }
                viewModel.FormModelState = modelState;
                viewModel.IsSignupSuccessfull = modelState.IsValid;
                if (viewModel.IsSignupSuccessfull)
                {
                    // Реєструємо у БД
                    Guid userId = Guid.NewGuid();
                    _dataContext.UsersData.Add(new()
                    {
                        Id = userId,
                        Name = viewModel.FormModel!.UserName,
                        Email = viewModel.FormModel!.UserEmail,
                        Birthdate = viewModel.FormModel!.UserBirthdate!.Value,
                    });
                    String salt = Guid.NewGuid().ToString();
                    _dataContext.UserAccesses.Add(new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        UserRoleId = _dataContext.UserRoles.First(r => r.Name == "Self Registered").Id,
                        Salt = salt,
                        Dk = _kdfService.Dk(salt, viewModel.FormModel!.UserPassword),
                        Login = viewModel.FormModel!.UserLogin,
                        AvatarFilename = viewModel.FormModel!.SavedFilename,
                        CreatedAt = DateTime.Now,
                    });
                    _dataContext.SaveChanges();
                }

                HttpContext.Session.Remove(nameof(UserSignupFormModel));
                HttpContext.Session.Remove(nameof(ModelState));
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SignUpForm(UserSignupFormModel formModel)
        {
            // Перевірка мінімального віку - 3650 = 10 років
            if (formModel.UserBirthdate != null && (DateTime.Now - formModel.UserBirthdate!.Value).Days < 3650)
            {
                ModelState.AddModelError("user-birthdate", "Вік замалий для реєстрації");
            }
            // Валідація паролю - ДЗ
            if (formModel.UserPassword != formModel.UserRepeat)
            {
                ModelState.AddModelError("user-repeat", "Повтор не збігається з паролем");
            }

            if (formModel.UserLogin != null)
            {
                if (_dataContext.UserAccesses.Any(ua => ua.Login == formModel.UserLogin))
                {
                    ModelState.AddModelError("user-login", "Даний логін вже у вжитку");
                }
            }

            if (ModelState.IsValid && formModel.UserAvatar != null && formModel.UserAvatar.Length > 0)
            {
                /* Д.З. Забезпечити валідацію файлу-аватарки
                 * на предмет того, що його розширення відповідає
                 * графічним файлам. Перелік узгодити з вибором MIME 
                 * типів у контролері Storage.
                 * Якщо файл має неприпустимий тип, то додавати 
                 * помилку валідації даного поля та виводити її на формі.
                 */
                formModel.SavedFilename = _storageService.Save(formModel.UserAvatar);
            }

            HttpContext.Session.SetString(
                nameof(ModelState),
                JsonSerializer.Serialize(ModelState)
            );

            HttpContext.Session.SetString(
                nameof(UserSignupFormModel),
                JsonSerializer.Serialize(formModel)
            );
            return RedirectToAction(nameof(SignUp));
        }

        [HttpGet]
        public JsonResult SignIn([FromRoute] String? id)   // id - з патерну у Program.cs
        {
            // Basic authentication
            String authHeader = Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(authHeader))
            {
                return Json(new
                {
                    status = 401,
                    data = "Missing 'Authorization' header"
                });
            }
            String scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                return Json(new
                {
                    status = 401,
                    data = "Invalid 'Authorization' scheme. Must be " + scheme
                });
            }
            String basicCredentials = authHeader[scheme.Length..];
            String userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(
                Convert.FromBase64String(basicCredentials));
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    status = 401,
                    data = "Credentials decode error " + ex.Message
                });
            }
            String[] parts = userPass.Split(':', 2);
            if(parts.Length != 2)
            {
                return Json(new
                {
                    status = 401,
                    data = "user-pass invalid format. Missing ':'? "
                });
            }

            UserAccess? userAccess = _dataContext
                .UserAccesses
                .Include(u => u.UserData)
                .FirstOrDefault(u => u.Login == parts[0]);

            if(userAccess == null)
            {
                return Json(new
                {
                    status = 401,
                    data = "Authentication rejected"
                });
            }

            if(_kdfService.Dk(userAccess.Salt, parts[1]) != userAccess.Dk)
            {
                return Json(new
                {
                    status = 401,
                    data = "Authentication rejected."
                });
            }

            if(id == "jwt")
            {
                return Json(new
                {
                    status = 200,
                    data = new JwtModel
                    {
                        Payload = new()
                        {
                            Name = userAccess.UserData.Name,
                            Email = userAccess.UserData.Email,
                            Aud = userAccess.UserRoleId == Guid.Parse("250FA2D3-0818-42D6-A1ED-112F115407D6")
        ? "Admin"
        : "Guest",
                            Sub = userAccess.Login,
                            Dob = userAccess.UserData.Birthdate.ToShortDateString(),
                            Iat = DateTime.Now.Ticks,
                            Ava = userAccess.AvatarFilename == null ? null :
        _storageService.GetPathPrefix() + userAccess.AvatarFilename,
                            Exp = DateTime.Now.AddMinutes(1000).Ticks,
                            Jti = userAccess.Id.ToString(),
                        }
                    }.ToString()
                });
            }
            else
            {
                HttpContext.Session.SetString("UserAccess", JsonSerializer.Serialize(userAccess));
                return Json(new
                {
                    status = 200,
                    data = "OK"
                });
            }
            

            /* Авторизація. Збереження результатів автентифікації.
             * За успішними результатами автентифікації у сесії зберігається
             * інформація про вхід.
             * У майбутніх запитах цю інформацію слід відновлювати
             * та приймати рішення щодо авторизації
             */
        }
    }
}
/* Д.З. Реалізувати стилізацію посилання переходу на 
 * сторінку профілю користувача в залежності від 
 * наявності/відсутності картинки-аватарки.
 * Додати підтвердження виходу з авторизованого 
 * режиму окремим модальним діалогом
 * [Ви виходите з системи
 *  Підтвержуєте
 *  Так   Ні      ]
 */
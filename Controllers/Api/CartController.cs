using AspKnp231.Models.Api.Cart;
using AspKnP231.Data;
using AspKnP231.Data.Entities;
using AspKnP231.Middleware.Auth.Token;
using AspKnP231.Models.Api;
using AspKnP231.Services.Storage;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;

namespace AspKnp231.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataContext dataContext, IStorageService storageService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;
        private RestResponse restResponse = new()
        {
            Meta = new()
            {
                ServerTime = DateTime.Now.Ticks,
                Cache = 0,
                DataType = "null",
                Path = "",
                Service = "Asp-Shop API"
            }
        };
        private UserAccess? CheckAuth()
        {


            if (!(HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                restResponse.Data =
                    HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? string.Empty;

                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }

            String userLogin = HttpContext.User.Claims
                .First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            UserAccess? userAccess = _dataContext.UserAccesses.FirstOrDefault(a => a.Login == userLogin);
            if (userAccess == null)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                restResponse.Data = "'Sub' not found";
                return null;
            }
            return userAccess;
        }
        [HttpGet("{id}")]
        public RestResponse LoadOrderDetails([FromRoute] String id)
        {
            restResponse.Meta.ResourceId = id;

            if (!Guid.TryParse(id, out Guid orderId))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                restResponse.Data = "Некоректний формат id. Очікується UUID";
                restResponse.Meta.DataType = "string";
                return restResponse;
            }

            if (CheckAuth() is UserAccess userAccess)
            {
                var cart = _dataContext
                    .Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .AsNoTracking()
                    .FirstOrDefault(c => c.Id == orderId && c.UserId == userAccess.UserId);

                if (cart == null)
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    restResponse.Data = "Замовлення не знайдено";
                    restResponse.Meta.DataType = "string";
                    return restResponse;
                }

                cart = cart with
                {
                    CartItems = [..cart.CartItems.Select(ci => ci with
            {
                Product = ci.Product with
                {
                    ImageUrl = _storageService.GetPathPrefix() +
                        (ci.Product.ImageUrl ?? "no_image.webp")
                }
            })]
                };

                restResponse.Data = cart;
                restResponse.Meta.DataType = "object";
            }

            return restResponse;
        }
        /* Д.З. Реалізувати перевірку вхідних та вилучених даних методів
         * LoadHistory: до метаданих додати відомості про загальну кількість
         *  наявних позицій
         * LoadOrderDetails: додати перевірку параметра id на формат UUID,
         *  а також встановлювати статус 404 якщо замовлення не буде знайдено
         */

        [HttpGet]
        public RestResponse LoadHistory()
        {
            UserAccess? userAccess = CheckAuth();
            if (userAccess == null) { return restResponse; }

            var carts = _dataContext
                .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .Where(c => c.UserId == userAccess.UserId)
                .AsNoTracking()
                .ToList();

            restResponse.Data = carts;
            restResponse.Meta.DataType = "array";
            restResponse.Meta.TotalCount = carts.Count;

            return restResponse;
        }


        [HttpPost]
        public RestResponse CreateOrder([FromBody] CartFormModel formModel)
        {
            UserAccess? userAccess = CheckAuth();
            if (userAccess == null) { return restResponse; }

            if (formModel.CartItems == null || formModel.CartItems.Length == 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                restResponse.Data = "Кошик порожній";
                return restResponse;
            }

            using var transaction = _dataContext.Database.BeginTransaction();

            try
            {
                var requestedProductIds = formModel.CartItems
                    .Select(ci => Guid.Parse(ci.ProductId))
                    .ToList();

                var products = _dataContext.ShopProducts
                    .Where(p => requestedProductIds.Contains(p.Id) && p.DeletedAt == null)
                    .ToList();

                foreach (var cartItem in formModel.CartItems)
                {
                    Guid productId;
                    if (!Guid.TryParse(cartItem.ProductId, out productId))
                    {
                        Response.StatusCode = StatusCodes.Status400BadRequest;
                        restResponse.Data = $"Некоректний ProductId: {cartItem.ProductId}";
                        transaction.Rollback();
                        return restResponse;
                    }

                    if (cartItem.Cnt <= 0)
                    {
                        Response.StatusCode = StatusCodes.Status400BadRequest;
                        restResponse.Data = $"Некоректна кількість товару: {cartItem.Cnt}";
                        transaction.Rollback();
                        return restResponse;
                    }

                    var product = products.FirstOrDefault(p => p.Id == productId);
                    if (product == null)
                    {
                        Response.StatusCode = StatusCodes.Status404NotFound;
                        restResponse.Data = $"Товар не знайдено: {cartItem.ProductId}";
                        transaction.Rollback();
                        return restResponse;
                    }

                    if (product.Stock < cartItem.Cnt)
                    {
                        Response.StatusCode = StatusCodes.Status409Conflict;
                        restResponse.Data = $"Недостатньо товару на складі: {product.Title}. Доступно {product.Stock}, потрібно {cartItem.Cnt}";
                        transaction.Rollback();
                        return restResponse;
                    }
                }

                Cart? cart = _dataContext.Carts
                    .FirstOrDefault(c => c.UserId == userAccess.UserId && c.OrderDt == null && c.DeleteDt == null);

                if (cart == null)
                {
                    cart = new Cart()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userAccess.UserId,
                        CreateDt = DateTime.Now,
                    };

                    _dataContext.Carts.Add(cart);
                }

                foreach (var cartItem in formModel.CartItems)
                {
                    var productId = Guid.Parse(cartItem.ProductId);
                    var product = products.First(p => p.Id == productId);

                    _dataContext.CartItems.Add(new CartItem()
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = cartItem.Cnt,
                        Price = (decimal)cartItem.Price
                    });

                    product.Stock -= cartItem.Cnt;
                }

                cart.Price = (decimal)formModel.Price;
                cart.OrderDt = DateTime.Now;

                _dataContext.SaveChanges();
                transaction.Commit();

                restResponse.Data = "Created";
                return restResponse;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                restResponse.Data = ex.Message;
                return restResponse;
            }
        }
    }
}
using AspKnP231.Models.User;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AspKnP231.Middleware.Auth.Token
{
    public class AuthTokenMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            JwtPayload jwtPayload;
            try
            {
                jwtPayload = GetJwtPayload(context);

                // Перевірити часові параметри
                if (jwtPayload.Exp != null)
                {
                    if (jwtPayload.Exp.Value < DateTime.Now.Ticks)
                    {
                        throw new Exception("Token expired");
                    }
                }

                context.User = new ClaimsPrincipal(

                       new ClaimsIdentity(
[
    new Claim(ClaimTypes.Name, jwtPayload.Name),
    new Claim(ClaimTypes.Email, jwtPayload.Email),
    new Claim(ClaimTypes.NameIdentifier, jwtPayload.Sub!),
    new Claim(ClaimTypes.Thumbprint, jwtPayload.Ava ?? ""),
    new Claim(ClaimTypes.DateOfBirth, jwtPayload.Dob),
    new Claim(ClaimTypes.Role, jwtPayload.Aud ?? "")
],
                    nameof(AuthTokenMiddleware)
                ));
            }
            catch (Exception ex)
            {
                context.Items.Add(nameof(AuthTokenMiddleware), ex.Message);
            }

        await _next(context);
        }
        private string UTF8FromBase64(String base64string)=>
            Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(base64string));
        private JwtPayload GetJwtPayload(HttpContext context)
        {
            String authHeader = context.Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(authHeader))
            {
                throw new Exception("Missing 'Authorization' header");
            }
            String scheme = "Bearer ";
            if (!authHeader.StartsWith(scheme))
            {
                throw new Exception("Invalid 'Authorization' scheme. Must be " + scheme);
            }
            String token = authHeader[scheme.Length..];
            // Валідація токена за https://datatracker.ietf.org/doc/html/rfc7519#section-7.2
            int dotPosition = token.IndexOf('.');
            if (dotPosition == -1)
            {
                throw new Exception("The JWT must contain at least one period ('.') character ");
            }
            String header = token[..dotPosition];

            // Base64url decode the Encoded JOSE Header following the
            // restriction that no line breaks, whitespace, or other additional
            // characters have been used. Verify that the resulting octet sequence is a UTF-8-encoded...
            String decodedHeader;
            try
            {
                decodedHeader = UTF8FromBase64(header);
            }
            catch
            {
                throw new Exception("The JWT header decode error ");
            }

            // 
            JwtHeader jwtHeader;
            try
            {
                jwtHeader = JsonSerializer.Deserialize<JwtHeader>(decodedHeader, JwtModel.options)!;
            }
            catch
            {
                throw new Exception("The JWT header must carry valid JSON");
            }
            if (jwtHeader.Typ != "JWT")
            {
                throw new Exception("The JWT header.typ unsupported: 'JWT' only");
            }
            if (jwtHeader.Alg != "HS256")
            {
                throw new Exception("The JWT header.alg unsupported: 'HS256' only");
            }

            // Відокремлюємо підпис та підписану частину
            dotPosition = token.LastIndexOf('.');
            String signedPart = token[..dotPosition];
            String signature = token[(dotPosition + 1)..];

            // Перевіряємо підпис шляхом нового підписування 
            String jwtSignature = JwtModel.Sign64(signedPart);
            if (jwtSignature != signature)
            {
                throw new Exception("Signature error");
            }

            // Відокремлюємо дані та декодуємо їх
            return JsonSerializer.Deserialize<JwtPayload>(
                UTF8FromBase64(signedPart.Split('.')[1]), JwtModel.options)!;
        }
    }
}
/*
            Client(Request)
/Home/Privacy    |        /api/cart
 Session      Middleware    JWT
Access->Claims   |       Payload->Claims
     |          / \          | 
  (Home)     Controllers   (Cart)
     |        /\    /\       |
     |      Views   API      | 
    Auth  (Privacy)  (Cart)  Auth
  (Claims)                  (Claims)
 
 */
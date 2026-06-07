namespace AspKnP231.Middleware.Auth.Token
{
    public static class AuthTokenExtension
    {
        public static IApplicationBuilder UseAuthToken(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthTokenMiddleware>();
        }
    }
}

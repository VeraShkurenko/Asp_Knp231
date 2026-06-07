namespace AspKnP231.Middleware.Auth.Session
{
    public static class AuthSessionExtension
    {
        public static IApplicationBuilder UseAuthSession(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthSessionMiddleware>();
        }
    }
}

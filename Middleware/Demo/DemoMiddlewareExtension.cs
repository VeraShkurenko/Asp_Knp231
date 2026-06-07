namespace AspKnP231.Middleware.Demo
{
    public static class DemoMiddlewareExtension
    {
        public static IApplicationBuilder UseDemo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DemoMiddleware>();
        }
    }
}

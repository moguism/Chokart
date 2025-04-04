using Microsoft.Extensions.Primitives;

namespace server;

public class PreAuthMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Console.WriteLine("entra middleware " + context.Request.Path);
        if (context.WebSockets.IsWebSocketRequest) 
        {
            Console.WriteLine("es peticion socket");

            context.Request.Method = HttpMethods.Get;

            if (context.Request.Query.TryGetValue("token", out StringValues jwt))
            {
                Console.WriteLine("tiene token " + jwt);

                context.Request.Headers.Authorization = $"Bearer {jwt}";

            
            }
        }

        return next(context);
    }
}

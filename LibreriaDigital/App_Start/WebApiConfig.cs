using LibreriaDigital.Filters;
using LibreriaDigital.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LibreriaDigital
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new JwtAuthenticationHandler());
            config.Filters.Add(new RateLimitAttribute());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AgencePlacementUi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Remove the existing default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Pages", action = "Index", id = UrlParameter.Optional }
            );

            // Add the new default route for Pages/Index.cshtml
            routes.MapRoute(
                name: "PagesDefault",
                url: "Pages/{action}/{id}",
                defaults: new { controller = "Pages", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Library.Models;
using System.Data.Entity;

namespace Library
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            logger.Info("Application Start");
            
            

            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        public override void Init()
        {
            logger.Info("Application Init");
        }

        public override void Dispose() 
        {
            logger.Info("Application Dispose");
        }

        protected void Application_Error()
        {
            logger.Info("Application Error");
        }


        protected void Application_End()
        {
            logger.Info("Application End");
        }
    }
}

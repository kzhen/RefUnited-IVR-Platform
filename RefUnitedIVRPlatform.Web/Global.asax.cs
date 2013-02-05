using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using RefUnitedIVRPlatform.Common.Interfaces;
using Autofac.Integration.WebApi;
using RefUnitedIVRPlatform.Business.IVRLogic;
using RefUnitedIVRPlatform.Business.Managers;
using RefUnitedIVRPlatform.Data.Repositories;

namespace RefUnitedIVRPlatform.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    private IContainer BuildMVCContainer()
    {
      var builder = new ContainerBuilder();

      builder.Register<IProfileRepository>(m => new ProfileRepositoryInMemory()).SingleInstance();
      builder.Register<IProfileManager>(m => new ProfileManager(m.Resolve<IProfileRepository>())).SingleInstance();
      builder.Register<IRefugeesUnitedAccountManager>(m => new RefugeesUnitedAccountManager()).InstancePerHttpRequest();
      builder.Register<IIVREntryLogic>(m => new IVREntryLogic(m.Resolve<IProfileManager>())).InstancePerHttpRequest();

      builder.RegisterControllers(typeof(MvcApplication).Assembly);
      builder.RegisterApiControllers(typeof(MvcApplication).Assembly);

      return builder.Build();
    }

    protected void Application_Start()
    {
      var container = BuildMVCContainer();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

      var resolver = new AutofacWebApiDependencyResolver(container);
      GlobalConfiguration.Configuration.DependencyResolver = resolver;

      AreaRegistration.RegisterAllAreas();

      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      AuthConfig.RegisterAuth();
    }
  }
}
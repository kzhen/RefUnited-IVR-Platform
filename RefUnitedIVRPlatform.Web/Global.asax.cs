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
using RefugeesUnitedApi;
using RefUnitedIVRPlatform.Business.SMSReceiverLogic;
using System.Configuration;

namespace RefUnitedIVRPlatform.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    private IContainer BuildMVCContainer()
    {
      var builder = new ContainerBuilder();

      var apiRequestSettings = new ApiRequestSettings()
      {
        Host = ConfigurationManager.AppSettings["RefUnitedApiHostName"],
        UserName = ConfigurationManager.AppSettings["RefUnitedApiUserName"],
        Password = ConfigurationManager.AppSettings["RefUnitedApiPassword"]
      };

      string twilioAccountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
      string twilioAuthToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
      string twilioPhoneNumber = ConfigurationManager.AppSettings["TwilioPhoneNumber"];

      var azureTableStorageConnectionString = ConfigurationManager.AppSettings["AzureTableStorageConnectionString"];

      builder.Register<IApiRequest>(m => new ApiRequest(apiRequestSettings)).InstancePerHttpRequest();
      builder.Register<IProfileRepository>(m => new ProfileRepositoryAzure(azureTableStorageConnectionString)).SingleInstance();

      builder.Register<IRefugeesUnitedAccountManager>(m => new RefugeesUnitedAccountManager(m.Resolve<IApiRequest>())).InstancePerHttpRequest();
      builder.Register<IProfileManager>(m => new ProfileManager(m.Resolve<IProfileRepository>())).SingleInstance();
      
      builder.Register<ISMSReceiverLogic>(m=>new SMSReceiverLogic(twilioAccountSid, twilioAuthToken, twilioPhoneNumber)).InstancePerHttpRequest();

      builder.Register<IIVREntryLogic>(m => new IVREntryLogic(m.Resolve<IProfileManager>())).InstancePerHttpRequest();
      builder.Register<IIVRMainLogic>(m => new IVRMainLogic(m.Resolve<IProfileManager>(), m.Resolve<IRefugeesUnitedAccountManager>()));
      builder.Register<IIVRAuthenticateLogic>(m => new IVRAuthenticateLogic(m.Resolve<IProfileManager>()));

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
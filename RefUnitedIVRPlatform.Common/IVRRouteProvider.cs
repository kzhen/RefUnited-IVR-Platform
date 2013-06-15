using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Attributes;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;

namespace RefUnitedIVRPlatform.Common
{
  public class IVRRouteProvider : IIVRRouteProvider
  {
    private static Dictionary<string, string> routes;
    private static List<Type> ignoredTypes;

    public IVRRouteProvider()
    {
      ignoredTypes = new List<Type>();
      ignoredTypes.Add(typeof(VoiceRequest));

      routes = new Dictionary<string, string>();

      Assembly.GetCallingAssembly().GetTypes()
        .SelectMany(t => t.GetMethods())
        .Where(m => m.GetCustomAttributes(typeof(IVRUrlRoute), false).Length > 0)
        .ToList()
        .ForEach(x =>
        {
          var ivrUrlRouteAttribute = ((IVRUrlRoute)System.Attribute.GetCustomAttribute(x, typeof(IVRUrlRoute)));

          StringBuilder sb = new StringBuilder();

          int controllerIdx = x.DeclaringType.Name.IndexOf("Controller");

          sb.Append("/");
          sb.Append(x.DeclaringType.Name.Substring(0, controllerIdx));
          sb.Append("/");
          sb.Append(x.Name);

          int paramCount = 0;

          x.GetParameters().ToList().ForEach(p =>
            {
              if (ignoredTypes.Contains(p.ParameterType))
              {
                return;
              }

              if (paramCount == 0)
              {
                sb.Append("?");
              }
              else
              {
                sb.Append("&");
              }

              sb.Append(p.Name);
              sb.Append("={" + paramCount++ + "}");
            });

          routes.Add(ivrUrlRouteAttribute.IVRRouteName, sb.ToString());
        });
    }

    public string GetUrlMethod(string ivrRoute, params object[] parameters)
    {
      string url = routes[ivrRoute];

      if (parameters.Length > 0)
      {
        url = string.Format(url, parameters);
      }

      return url;
    }
  }
}

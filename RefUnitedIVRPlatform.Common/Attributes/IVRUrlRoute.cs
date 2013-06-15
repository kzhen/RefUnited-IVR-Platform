using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Attributes
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public sealed class IVRUrlRoute : Attribute
  {
    public IVRUrlRoute(string routeName)
    {
      IVRRouteName = routeName;
    }

    public string IVRRouteName
    {
      get;
      private set;
    }
  }
}

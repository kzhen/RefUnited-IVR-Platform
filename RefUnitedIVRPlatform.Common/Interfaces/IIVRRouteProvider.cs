using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IIVRRouteProvider
  {
    string GetUrlMethod(string ivrRoute, params object[] parameters);
  }
}

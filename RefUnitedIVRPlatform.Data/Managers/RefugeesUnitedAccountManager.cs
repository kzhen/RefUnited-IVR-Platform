using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefugeesUnitedApi;
using RefUnitedIVRPlatform.Common.Interfaces;

namespace RefUnitedIVRPlatform.Data.Managers
{
  public class RefugeesUnitedAccountManager : IRefugeesUnitedAccountManager
  {
    private ApiRequestSettings apiRequestSettings = new ApiRequestSettings()
    {
    };

    public bool ValidateLogin(string username, string password)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var loginRequest = request.ProfileLogin(username, password);

      if (loginRequest == null)
      {
        return false;
      }

      return loginRequest.Authenticated;
    }
  }
}

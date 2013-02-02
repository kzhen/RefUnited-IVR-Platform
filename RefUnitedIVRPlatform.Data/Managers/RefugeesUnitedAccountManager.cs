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
      Host = "http://api.ru.istykker.dk/",
      UserName = "hackathon",
      Password = "179d50c6eb31188925926a5d1872e8117dc58572"
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

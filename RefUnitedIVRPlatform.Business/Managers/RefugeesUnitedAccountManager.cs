using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefugeesUnitedApi;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefugeesUnitedApi.ApiEntities;

namespace RefUnitedIVRPlatform.Business.Managers
{
  public class RefugeesUnitedAccountManager : IRefugeesUnitedAccountManager
  {
    private static int FAVOURITE_PAGE_SIZE = 9;

    private ApiRequestSettings apiRequestSettings = new ApiRequestSettings()
    {
    };

    public ProfileLoginResult ValidateLogin(string username, string password)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var loginRequest = request.ProfileLogin(username, password);

      return loginRequest;
    }

    public Profile GetProfile(int profileId)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var profile = request.GetProfile(profileId);

      return profile;
    }

    public void UpdateProfile(Profile profile)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      request.UpdateProfile(profile);
    }

    public int GetUnreadMessageCount(int profileId)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var result = request.GetUnreadMessages(profileId);

      return result.UnreadMessages;
    }

    public List<Profile> GetFavourites(int profileId, int pageIdx)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var results = request.GetFavourites(profileId);

      if (results.Count > FAVOURITE_PAGE_SIZE)
      {
        if (pageIdx == 0)
        {
          return results.Take(FAVOURITE_PAGE_SIZE).ToList();
        }
        else
        {
          int numToSkip = FAVOURITE_PAGE_SIZE * pageIdx;

          return results.Skip(numToSkip).Take(FAVOURITE_PAGE_SIZE).ToList();
        }
      }

      return results;
    }

    public ProfileMessageCollection GetMessages(int profileId)
    {
      ApiRequest request = new ApiRequest(apiRequestSettings);

      var results = request.GetMessageCollection(profileId);

      return results;
    }
  }
}

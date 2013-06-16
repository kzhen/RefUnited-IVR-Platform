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
    private IApiRequest apiRequest;

    public RefugeesUnitedAccountManager(IApiRequest apiRequest)
    {
      this.apiRequest = apiRequest;
    }

    public ProfileLoginResult ValidateLogin(string username, string password)
    {
      var loginRequest = apiRequest.ProfileLogin(username, password);

      return loginRequest;
    }

    public Profile GetProfile(int profileId)
    {
      var profile = apiRequest.GetProfile(profileId);

      return profile;
    }

    public void UpdateProfile(Profile profile)
    {
      apiRequest.UpdateProfile(profile);
    }

    public int GetUnreadMessageCount(int profileId)
    {
      var result = apiRequest.GetUnreadMessages(profileId);

      return result.UnreadMessages;
    }

    public List<Profile> GetFavourites(int profileId, int pageIdx)
    {
      var results = apiRequest.GetFavourites(profileId);

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
      var results = apiRequest.GetMessageCollection(profileId);

      return results;
    }


    public void AddFavourite(int profileId, int targetProfileId)
    {
      var results = apiRequest.AddProfileFavourite(profileId, targetProfileId);
    }
  }
}

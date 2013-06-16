using RefugeesUnitedApi.ApiEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IRefugeesUnitedAccountManager
  {
    ProfileLoginResult ValidateLogin(string username, string password);
    Profile GetProfile(int profileId);
    void UpdateProfile(Profile profile);
    int GetUnreadMessageCount(int profileId);
    List<Profile> GetFavourites(int profileId, int pageIdx);
    ProfileMessageCollection GetMessages(int profileId);
    void AddFavourite(int profileId, int profileIdToFavourite);
  }
}

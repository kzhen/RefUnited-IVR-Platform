using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IProfileManager
  {
    void CreateProfile(IVRProfile profile);
    IVRProfile GetProfile(int profileId);
    void UpdateProfile(IVRProfile profile);

    bool CheckPin(string lookupPhoneNumber, string pin);
    bool CheckNumber(string lookupPhoneNumber);
    
    string GetPin(string lookupPhoneNumber);
    int GetProfileId(string lookupPhoneNumber);

    List<Recording> GetRecordings();
    List<Recording> GetRecordings(int profileId);
    
    void SaveRecording(int profileId, int targetProfileId, string url);
    string GetCulture(string lookupPhoneNumber);

    IVRProfile GetProfileByPhoneNumber(string phoneNumber);

    void DeleteRecording(int profileId, int recordingIdx);

    List<IVRProfile> GetAllProfiles();

    bool CheckIfProfileExists(int profileId);
  }
}

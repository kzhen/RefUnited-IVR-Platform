using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Business.Managers
{
  public class ProfileManager : IProfileManager
  {
    private List<String> urls;
    private List<Recording> recordings;
    private IProfileRepository profileRepository;

    public ProfileManager(IProfileRepository repository)
    {
      urls = new List<string>();
      recordings = new List<Recording>();
      this.profileRepository = repository;
    }

    public void CreateProfile(IVRProfile profile)
    {
      profileRepository.Create(profile);
    }

    public IVRProfile GetProfile(int profileId)
    {
      return profileRepository.Get(profileId);
    }

    public void UpdateProfile(IVRProfile profile)
    {
      profileRepository.Update(profile);
    }

    public bool CheckNumber(string lookupPhoneNumber)
    {
      try
      {
        var profile = profileRepository.GetByPhoneNumber(lookupPhoneNumber);
        if (profile != null)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    public bool CheckPin(string lookupPhoneNumber, string pin)
    {
      var profile = profileRepository.GetByPhoneNumber(lookupPhoneNumber);

      return profile.PIN.Equals(pin, StringComparison.OrdinalIgnoreCase);
    }

    public string GetPin(string lookupPhoneNumber)
    {
      var profile = profileRepository.GetByPhoneNumber(lookupPhoneNumber);

      return profile.PIN;
    }

    public int GetProfileId(string lookupPhoneNumber)
    {
      var profile = profileRepository.GetByPhoneNumber(lookupPhoneNumber);

      return profile.ProfileId;
    }

    public void SaveRecording(string url)
    {
      urls.Add(url);
    }

    public List<string> GetRecordingUrls()
    {
      return this.urls;
    }

    public void SaveRecording(int profileId, int targetProfileId, string url)
    {
      Recording recording = new Recording()
      {
        FromProfileId = profileId,
        ToProfileId = targetProfileId,
        Url = url
      };

      recordings.Add(recording);
    }

    public List<Recording> GetRecordings()
    {
      return this.recordings;
    }

    public List<Recording> GetRecordings(int profileId)
    {
      return this.recordings.Where(m => m.ToProfileId == profileId).ToList();
    }

    public string GetCulture(string lookupPhoneNumber)
    {
      var profile = profileRepository.GetByPhoneNumber(lookupPhoneNumber);

      return profile.Culture;
    }

    public IVRProfile GetProfileByPhoneNumber(string phoneNumber)
    {
      return profileRepository.GetByPhoneNumber(phoneNumber);
    }
  }
}

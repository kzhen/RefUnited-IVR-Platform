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
    private IProfileRepository profileRepository;
    private IRecordingRepository recordingRepository;
    private IRefugeesUnitedAccountManager refUnitedAccountManager;

    public ProfileManager(IProfileRepository repository, IRecordingRepository recordingRepository, IRefugeesUnitedAccountManager refUnitedAccountManager)
    {
      this.profileRepository = repository;
      this.recordingRepository = recordingRepository;
      this.refUnitedAccountManager = refUnitedAccountManager;
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

    public void SaveRecording(int profileId, int targetProfileId, string url)
    {
      Recording recording = new Recording()
      {
        FromProfileId = profileId,
        ToProfileId = targetProfileId,
        Url = url
      };

      recordingRepository.Create(recording);
    }

    public List<Recording> GetRecordings()
    {
      return recordingRepository.GetAll();
    }

    public List<Recording> GetRecordings(int profileId)
    {
      return recordingRepository.GetForProfile(profileId);
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

    public void DeleteRecording(int profileId, int recordingId)
    {
      throw new NotImplementedException();
    }

    public List<IVRProfile> GetAllProfiles()
    {
      return this.profileRepository.GetAll();
    }

    public bool CheckIfProfileExists(int profileId)
    {
      try
      {
        var profile = profileRepository.Get(profileId);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }


    public void AddAsFavourite(int profileId, int profileIdToFavourite)
    {
      refUnitedAccountManager.AddFavourite(profileId, profileIdToFavourite);
    }
  }
}

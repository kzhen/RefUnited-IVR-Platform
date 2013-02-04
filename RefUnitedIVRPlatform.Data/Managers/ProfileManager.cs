using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.Managers
{
  public class ProfileManager : IProfileManager
  {
    private Dictionary<string, string> pins;
    private Dictionary<string, int> profileIds;
    private Dictionary<string, string> profileCultures;
    private List<String> urls;
    private List<Recording> recordings;

    public ProfileManager()
    {
      pins = new Dictionary<string, string>();
      profileIds = new Dictionary<string, int>();
      urls = new List<string>();
      profileCultures = new Dictionary<string, string>();

      recordings = new List<Recording>();
    }

    public bool CreatePin(string phoneNumber, string pin, int profileId)
    {
      profileIds[phoneNumber] = profileId;

      if (pins.ContainsKey(phoneNumber))
      {
        pins[phoneNumber] = pin;
        return true;
      }

      pins[phoneNumber] = pin;
      return false;
    }


    public bool CheckNumber(string lookupPhoneNumber)
    {
      return pins.ContainsKey(lookupPhoneNumber);
    }

    public bool CheckPin(string lookupPhoneNumber, string pin)
    {
      if (pins.ContainsKey(lookupPhoneNumber))
      {
        if (pins[lookupPhoneNumber].Equals(pin))
        {
          return true;
        }
      }
      return false;
    }


    public string GetPin(string lookupPhoneNumber)
    {
      return pins[lookupPhoneNumber];
    }


    public int GetProfileId(string lookupPhoneNumber)
    {
      if (profileIds.ContainsKey(lookupPhoneNumber))
      {
        return profileIds[lookupPhoneNumber];
      }
      else
      {
        throw new Exception("ProfileId not found...");
      }
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
      if (profileCultures.ContainsKey(lookupPhoneNumber))
        return profileCultures[lookupPhoneNumber];

      return null;
    }


    public void SetLanguage(string phoneNumber, string language)
    {
      profileCultures[phoneNumber] = language;
    }
  }
}

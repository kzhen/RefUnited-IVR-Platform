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
    private List<String> urls;

    public ProfileManager()
    {
      pins = new Dictionary<string, string>();
      profileIds = new Dictionary<string, int>();
      urls = new List<string>();
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


    public List<string> GetRecordings()
    {
      return this.urls;
    }
  }
}

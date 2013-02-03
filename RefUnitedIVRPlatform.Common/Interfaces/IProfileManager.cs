﻿using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IProfileManager
  {
    bool CreatePin(string phoneNumber, string pin, int profileId);
    bool CheckNumber(string lookupPhoneNumber);
    bool CheckPin(string lookupPhoneNumber, string pin);
    string GetPin(string lookupPhoneNumber);
    int GetProfileId(string lookupPhoneNumber);
    void SaveRecording(string url);
    List<string> GetRecordingUrls();
    List<Recording> GetRecordings();
    List<Recording> GetRecordings(int profileId);
    void SaveRecording(int profileId, int targetProfileId, string url);
  }
}

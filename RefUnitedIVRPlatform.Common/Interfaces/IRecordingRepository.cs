using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IRecordingRepository : IRepository<Recording>
  {
    List<Recording> GetForProfile(int profileId);

    void DeleteById(int recordingId);
  }
}

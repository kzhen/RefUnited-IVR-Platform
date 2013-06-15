using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Interfaces;

namespace RefUnitedIVRPlatform.Business.Managers
{
  public class BroadcastManager : IBroadcastManager
  {
    public void SaveBroadcast(Common.Entities.PublicBroadcast broadcast)
    {
      throw new NotImplementedException();
    }

    public List<Common.Entities.PublicBroadcast> GetAll()
    {
      throw new NotImplementedException();
    }

    public List<Common.Entities.PublicBroadcast> GetSimiliar(int profileId)
    {
      throw new NotImplementedException();
    }
  }
}

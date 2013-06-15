using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;

namespace RefUnitedIVRPlatform.Business.Managers
{
  public class BroadcastManager : IBroadcastManager
  {
    private List<PublicBroadcast> publicBroadcasts;

    public BroadcastManager()
    {
      publicBroadcasts = new List<PublicBroadcast>();
    }

    public void SaveBroadcast(PublicBroadcast broadcast)
    {
      publicBroadcasts.Add(broadcast);
    }

    public List<PublicBroadcast> GetAll()
    {
      return publicBroadcasts;
    }

    public List<PublicBroadcast> GetSimiliar(int profileId)
    {
      throw new NotImplementedException();
    }
  }
}

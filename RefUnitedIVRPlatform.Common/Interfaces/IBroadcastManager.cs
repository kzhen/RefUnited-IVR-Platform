using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Entities;

namespace RefUnitedIVRPlatform.Common.Interfaces
{
  public interface IBroadcastManager
  {
    void SaveBroadcast(PublicBroadcast broadcast);
    List<PublicBroadcast> GetAll();
    List<PublicBroadcast> GetSimiliar(int profileId);

    PublicBroadcast Get(int lastBroadcastIdx);

    void SaveBroadcastReply(PublicBroadcast broadcast, PublicBroadcast broadcastReply);
  }
}

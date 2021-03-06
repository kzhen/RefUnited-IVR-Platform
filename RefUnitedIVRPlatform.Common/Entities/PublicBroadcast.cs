﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Entities
{
  public class PublicBroadcast
  {
    public int FromProfileId { get; set; }
    public string Url { get; set; }
    public List<PublicBroadcast> BroadcastReplies { get; private set; }

    public PublicBroadcast()
    {
      BroadcastReplies = new List<PublicBroadcast>();
    }
  }
}

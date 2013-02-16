using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RefUnitedIVRPlatform.Common.Entities
{
  public class IVRProfile
  {
    public string FullName { get; set; }
    public int ProfileId { get; set; }
    public string PhoneNumber { get; set; }
    public string PIN { get; set; }
    public string Culture { get; set; }
    public List<Recording> Recordings { get; set; }

    public IVRProfile()
    {
      this.Recordings = new List<Recording>();
    }
  }
}
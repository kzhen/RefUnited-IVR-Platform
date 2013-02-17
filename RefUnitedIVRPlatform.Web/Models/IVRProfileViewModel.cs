using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RefUnitedIVRPlatform.Web.Models
{
  public class IVRProfileViewModel
  {
    public IVRProfile Profile { get; set; }
    public bool CanEdit { get; set; }
  }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common.Entities
{
  public class Recording
  {
    public int RecordingId { get; set; }
    public int FromProfileId { get; set; }
    public int ToProfileId { get; set; }
    public string Url { get; set; }
  }
}

using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.DtoEntities
{
  internal class RecordingEntity : TableEntity
  {
    public int RecordingId { get; set; }
    public int FromProfileId { get; set; }
    public int ToProfileId { get; set; }
    public string Url { get; set; }

    public RecordingEntity()
    {
      this.PartitionKey = ToProfileId.ToString();
      this.RowKey = RecordingId.ToString();
    }
  }
}

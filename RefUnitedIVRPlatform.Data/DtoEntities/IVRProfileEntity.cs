using Microsoft.WindowsAzure.Storage.Table;
using RefUnitedIVRPlatform.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.DtoEntities
{
  internal class IVRProfileEntity : TableEntity
  {
    public string FullName { get; set; }
    public int ProfileId { get; set; }
    public string PhoneNumber { get; set; }
    public string PIN { get; set; }
    public string Culture { get; set; }

    public IVRProfileEntity()
    {
      this.PartitionKey = FullName;
      this.RowKey = ProfileId.ToString();
    }
  }
}

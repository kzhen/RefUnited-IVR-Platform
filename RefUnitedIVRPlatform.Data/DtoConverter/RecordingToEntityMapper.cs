using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Data.DtoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.DtoConverter
{
  internal static class RecordingToEntityMapper
  {
    internal static RecordingEntity ConvertToEntity(Recording recordingToConvert)
    {
      return new RecordingEntity()
      {
        FromProfileId = recordingToConvert.FromProfileId,
        ToProfileId = recordingToConvert.ToProfileId,
        Url = recordingToConvert.Url,
        PartitionKey = recordingToConvert.ToProfileId.ToString(),
        RowKey = Guid.NewGuid().ToString()
      };
    }

    internal static Recording ConvertFromEntity(RecordingEntity entityToConvert)
    {
      return new Recording()
      {
        FromProfileId = entityToConvert.FromProfileId,
        ToProfileId = entityToConvert.ToProfileId,
        Url = entityToConvert.Url
      };
    }

    internal static void UpdateEntity(IVRProfileEntity entityToUpdate, RecordingEntity profileEntity)
    {
      throw new NotImplementedException();
    }
  }
}

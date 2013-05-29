using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Data.DtoEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Data.DtoConverter
{
  internal static class IVRProfileToEntityMapper
  {
    internal static IVRProfileEntity ConvertToEntity(IVRProfile profile)
    {
      IVRProfileEntity ret = new IVRProfileEntity()
      {
        FullName = profile.FullName,
        Culture = profile.Culture,
        ProfileId = profile.ProfileId,
        PhoneNumber = profile.PhoneNumber,
        PIN = profile.PIN,

      };

      return ret;
    }
  }
}

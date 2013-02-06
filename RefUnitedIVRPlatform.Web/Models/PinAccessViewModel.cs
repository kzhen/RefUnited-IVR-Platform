using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RefUnitedIVRPlatform.Web.Models
{
  public class PinAccessViewModel
  {
    public int ProfileId { get; set; }
    public string DialCode { get; set; }
    public string CellPhoneNumber { get; set; }

    [Required]
    public string Language { get; set; }
    
    [Required]
    [MinLength(4)]
    [MaxLength(8)]
    public int PIN { get; set; }
  }
}
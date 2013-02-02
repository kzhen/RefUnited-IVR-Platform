using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class HomeController : Controller
  {
    private IProfileManager profileManager;

    public HomeController(IProfileManager profileManager)
    {
      this.profileManager = profileManager;
    }

    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Recordings()
    {
      var model = profileManager.GetRecordings();

      return View(model);
    }
  }
}

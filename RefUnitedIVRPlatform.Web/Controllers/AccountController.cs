using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using RefUnitedIVRPlatform.Web.Models;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Common.Entities;

namespace RefUnitedIVRPlatform.Web.Controllers
{
  public class AccountController : Controller
  {
    private IRefugeesUnitedAccountManager refUnitedAccountManager;
    private IProfileManager profileManager;

    public AccountController(IRefugeesUnitedAccountManager refUnitedManager, IProfileManager profileManager)
    {
      this.refUnitedAccountManager = refUnitedManager;
      this.profileManager = profileManager;
    }

    public ActionResult Login(string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    public ActionResult List()
    {
      var model = profileManager.GetAllProfiles();

      return View(model);
    }

    public ActionResult Info(int profileId)
    {
      if (Request.Cookies["RefUser"] != null && Request.Cookies["RefProfileId"] != null)
      {
        profileId = int.Parse(Request.Cookies["RefProfileId"].Value);
      }

      var model = profileManager.GetProfile(profileId);
      var recordings = profileManager.GetRecordings(profileId);
      model.Recordings = recordings;

      if (model == null)
      {
        return View("AccountNotFound");
      }

      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginModel model, string returnUrl)
    {
      var authResults = refUnitedAccountManager.ValidateLogin(model.UserName, model.Password);

      if (ModelState.IsValid && authResults.Authenticated)
      {
        var profile = refUnitedAccountManager.GetProfile(authResults.ProfileId);
        profile.ProfileId = authResults.ProfileId;

        var token = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(model.UserName, false, 60));
        Response.Cookies.Add(new HttpCookie("RefUser", token));

        return View("VerifyProfile", profile);
      }

      // If we got this far, something failed, redisplay form
      ModelState.AddModelError("", "The user name or password provided is incorrect.");
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult VerifyProfile(RefugeesUnitedApi.ApiEntities.Profile profile)
    {
      if (ModelState.IsValid)
      {
        //check if they have changed anything...
        //if(hasChanged)
        //{
        refUnitedAccountManager.UpdateProfile(profile);
        var profileModel = refUnitedAccountManager.GetProfile(profile.ProfileId);
        //} 
        //else 
        //{
        //
        //}
        PinAccessViewModel model = new PinAccessViewModel()
        {
          ProfileId = profileModel.ProfileId,
          DialCode = profileModel.DialCode,
          CellPhoneNumber = profileModel.CellPhoneNumber
        };

        return View("SetupPINAccess", model);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SetupPINAccess(FormCollection form)
    {
      int profileId = int.Parse(form["profileId"]);
      string pin = form["PIN"];
      string language = form["Language"];
      string phoneNumber = string.Format("{0}{1}", form["DialCode"], form["CellPhoneNumber"]);

      Response.Cookies.Add(new HttpCookie("RefProfileId", profileId.ToString()));

      var ivrProfile = new IVRProfile()
      {
        ProfileId = profileId,
        PIN = pin,
        Culture = language,
        PhoneNumber = phoneNumber
      };

      profileManager.CreateProfile(ivrProfile);

      return RedirectToAction("SetupComplete");
    }

    public ActionResult SetupComplete()
    {
      return View();
    }

    #region Helpers
    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    public enum ManageMessageId
    {
      ChangePasswordSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
    }

    internal class ExternalLoginResult : ActionResult
    {
      public ExternalLoginResult(string provider, string returnUrl)
      {
        Provider = provider;
        ReturnUrl = returnUrl;
      }

      public string Provider { get; private set; }
      public string ReturnUrl { get; private set; }

      public override void ExecuteResult(ControllerContext context)
      {
        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
      }
    }

    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "User name already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}

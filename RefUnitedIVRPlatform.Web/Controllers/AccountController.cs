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
using RefUnitedIVRPlatform.Web.Filters;
using RefUnitedIVRPlatform.Web.Models;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Data.Managers;
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

    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    public ActionResult Info(string phoneNumber)
    {
      int profileId;

      try
      {
        profileId = profileManager.GetProfileId(phoneNumber);
      }
      catch (Exception)
      {
        ViewBag.PhoneNumber = phoneNumber;
        return View("AccountNotFound");
      }

      IVRProfileViewModel model = new IVRProfileViewModel();
      model.ProfileId = profileId;
      model.Recordings = profileManager.GetRecordings(model.ProfileId);
      model.PIN = profileManager.GetPin(phoneNumber);
      model.PhoneNumber = phoneNumber;
      model.Culture = profileManager.GetCulture(phoneNumber);

      return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginModel model, string returnUrl)
    {
      var authResults = refUnitedAccountManager.ValidateLogin(model.UserName, model.Password);

      if (ModelState.IsValid && authResults.Authenticated)
      {
        var profile = refUnitedAccountManager.GetProfile(authResults.ProfileId);
        profile.ProfileId = authResults.ProfileId;

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
        var model = refUnitedAccountManager.GetProfile(profile.ProfileId);
        //} 
        //else 
        //{
        //
        //}

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

      var result = profileManager.CreatePin(phoneNumber, pin, profileId);

      profileManager.SetLanguage(phoneNumber, language);

      return RedirectToAction("SetupComplete", new { existingPin = result });
    }

    public ActionResult SetupComplete(bool existingPin)
    {
      return View(existingPin);
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

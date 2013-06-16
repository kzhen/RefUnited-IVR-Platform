using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common;
using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business.IVRLogic
{
  public class IVRBroadcastLogic : IIVRBroadcastLogic
  {
    private IBroadcastManager broadcastManager;
    private IIVRRouteProvider ivrRouteProvider;
    private IProfileManager profileManager;

    public IVRBroadcastLogic(IBroadcastManager broadcastManager, IIVRRouteProvider ivrRouteProvider, IProfileManager profileManager)
    {
      this.broadcastManager = broadcastManager;
      this.ivrRouteProvider = ivrRouteProvider;
      this.profileManager = profileManager;
    }

    public TwilioResponse RecordBroadcast(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      response.Say("At the tone record your message, to finish press any key. Please note that this will be public to all people on the Refugees United platform.");
      response.Record(new { action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_SAVE_PUBLIC_BROADCAST, profileId), playBeep = true });

      return response;
    }

    public TwilioResponse RecordBroadcast_SaveRecording(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      broadcastManager.SaveBroadcast(new PublicBroadcast()
      {
        FromProfileId = profileId,
        Url = request.RecordingUrl
      });

      response.Say("Thankyou, your broadcast has been sent.");
      response.Redirect("/IVRMain/MainMenu");

      return response;
    }

    public TwilioResponse ListenToBroadcastsMenu(VoiceRequest request, int profileId)
    {
      var response = new TwilioResponse();

      if (string.IsNullOrEmpty(request.Digits))
      {
        response.BeginGather(new { numDigits = 1, action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_MENU, profileId.ToString()) });
        response.Say("Press one to listen to all public broadcasts");
        //response.Say("Press two to listen to public broadcasts which match your country of origin");
        response.EndGather();

        return response;
      }

      switch (request.Digits)
      {
        case "1":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_LISTEN_TO_ALL_PUBLIC, profileId, 0));
          return response;
        case "2":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_LISTEN_TO_MATCHED, "countryOfBirthId"));
          return response;
        default:
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.MAIN_MENU_SELECTION));
          return response;
      }
    }

    public TwilioResponse ListenToPublicBroadcasts(VoiceRequest request, int profileId, int idx)
    {
      var response = new TwilioResponse();

      PublicBroadcast broadcast;

      try
      {
        broadcast = broadcastManager.GetAll().Skip(idx).Take(1).FirstOrDefault();
      }
      catch (Exception)
      {
        broadcast = null;
      }

      if (broadcast == null)
      {
        response.Say("No more broadcasts found, returning to the main menu");
        response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

        return response;
      }

      var fromProfile = profileManager.GetProfile(broadcast.FromProfileId);

      response.Say(string.Format("Playing broadcast from {0}", fromProfile.FullName));
      response.Play(broadcast.Url);

      response.BeginGather(new { action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_RESPONSE_SELECTION, profileId, idx), playBeep = true, numDigits = 1 });
      response.Say("Press one to reply privately to this broadcast.");
      response.Say("Press two to reply publicly to this broadcast.");
      response.Say("Press three to listen to the next broadcast.");

      if (broadcast.BroadcastReplies.Count > 0)
      {
        response.Say("Press four to listen to responses to this broadcast");
      }

      response.EndGather();
      
      return response;
    }

    public TwilioResponse BroadcastResponseSelection(VoiceRequest request, int profileId, int lastBroadcastIdx)
    {
      var response = new TwilioResponse();

      var selection = request.Digits;

      switch (selection)
      {
        case "1":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_REPLY_PRIVATELY, profileId, lastBroadcastIdx));
          return response;
        case "2":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_REPLY_PUBLICLY, profileId, lastBroadcastIdx));
          return response;
        case "4":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_PLAY_PUBLIC_REPLY, profileId, lastBroadcastIdx, 0));
          return response;
        case "3":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_LISTEN_TO_ALL_PUBLIC, profileId, ++lastBroadcastIdx));
          return response;
        default:
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));
          return response;
      }
    }

    public TwilioResponse RecordPrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = new TwilioResponse();
      response.Say("At the tone please record your reply.");
      response.Record(new { action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_SAVE_PRIVATE_REPLY, profileId, lastBroadcastIdx, subBroadcastIdx), playBeep = true });
      return response;
    }

    public TwilioResponse SavePrivateReply(VoiceRequest request, int profileId, int lastBroadcastIdx, int? subBroadcastIdx)
    {
      var response = new TwilioResponse();

      response.Say("Thank you, your response has been sent.");
      response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

      var broadcast = broadcastManager.Get(lastBroadcastIdx);

      int fromProfileId = broadcast.FromProfileId;

      if (subBroadcastIdx.HasValue)
      {
        var reply = broadcast.BroadcastReplies.Skip(subBroadcastIdx.Value).Take(1).SingleOrDefault();
        fromProfileId = reply.FromProfileId;
      }

      profileManager.SaveRecording(profileId, fromProfileId, request.RecordingUrl);

      return response;
    }

    public TwilioResponse RecordPublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx)
    {
      var response = new TwilioResponse();
      response.Say("At the tone please record your reply.");
      response.Record(new { action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_SAVE_PUBLIC_REPLY, profileId, lastBroadcastIdx), playBeep = true });
      return response;
    }

    public TwilioResponse SavePublicReply(VoiceRequest request, int profileId, int lastBroadcastIdx)
    {
      var response = new TwilioResponse();

      response.Say("Thank you, your response has been sent.");
      response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

      var broadcast = broadcastManager.Get(lastBroadcastIdx);

      PublicBroadcast broadcastReply = new PublicBroadcast()
      {
        FromProfileId = profileId,
        Url = request.RecordingUrl
      };

      broadcastManager.SaveBroadcastReply(broadcast, broadcastReply);

      return response;
    }

    public TwilioResponse ListenToMatchedBroadcasts(VoiceRequest request, int profileId)
    {
      throw new NotImplementedException();
    }

    public TwilioResponse ListenToBroadcastReplies(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      var response = new TwilioResponse();

      PublicBroadcast broadcast;

      try
      {
        broadcast = broadcastManager.GetAll().Skip(lastBroadcastIdx).Take(1).FirstOrDefault();
      }
      catch (Exception)
      {
        broadcast = null;
      }

      if (broadcast == null)
      {
        response.Say("Broadcast not found, returning to the main menu");
        response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

        return response;
      }

      if (broadcast.BroadcastReplies == null || broadcast.BroadcastReplies.Count == 0)
      {
        response.Say("Broadcast has no replies, returning to the main menu");
        response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

        return response;
      }

      var reply = broadcast.BroadcastReplies.Skip(subBroadcastIdx).Take(1).FirstOrDefault();

      if (reply == null)
      {
        response.Say("No more replies, returning to the main menu");
        response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

        return response;
      }

      var fromProfile = profileManager.GetProfile(reply.FromProfileId);

      response.Say(string.Format("Playing broadcast reply from {0}", fromProfile.FullName));
      response.Play(reply.Url);

      if (broadcast.BroadcastReplies.Count - subBroadcastIdx <= 0)
      {
        response.Say("That was the last reply to this broadcast.");
      }

      response.BeginGather(new { action = ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCAST_REPLY_RESPONSE_SELECTION, profileId, lastBroadcastIdx, subBroadcastIdx), playBeep = true, numDigits = 1 });
      response.Say("Press one to reply privately to this broadcast.");
      response.Say("Press two to reply publicly to this broadcast.");

      if (broadcast.BroadcastReplies.Count - subBroadcastIdx > 0)
      {
        response.Say("Press three to listen to the next broadcast reply");
      }

      response.Say("Press four to listen to the next public broadcast");

      response.Say("Press five to add person as a favourite");

      response.EndGather();

      return response;
    }

    public TwilioResponse BroadcastReplyMenuSelection(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      var response = new TwilioResponse();

      var selection = request.Digits;

      switch (selection)
      {
        case "1":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_REPLY_PRIVATELY, profileId, lastBroadcastIdx, subBroadcastIdx));
          return response;
        case "2":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_REPLY_PUBLICLY, profileId, lastBroadcastIdx));
          return response;
        case "3":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_PLAY_PUBLIC_REPLY, profileId, lastBroadcastIdx, ++subBroadcastIdx));
          return response;
        case "4":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_LISTEN_TO_ALL_PUBLIC, profileId, ++lastBroadcastIdx));
          return response;
        case "5":
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.BROADCASTS_ADD_REPLIER_AS_FAVOURITE, profileId, lastBroadcastIdx, subBroadcastIdx));
          return response;
        default:
          response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));
          return response;
      }
    }

    public TwilioResponse AddResponderAsFavourite(VoiceRequest request, int profileId, int lastBroadcastIdx, int subBroadcastIdx)
    {
      var broadcast = broadcastManager.GetAll().Skip(lastBroadcastIdx).Take(1).FirstOrDefault();
      var reply = broadcast.BroadcastReplies.Skip(subBroadcastIdx).Take(1).FirstOrDefault();

      int profileIdToFavourite = reply.FromProfileId;
      profileManager.AddAsFavourite(profileId, profileIdToFavourite);

      var profile = profileManager.GetProfile(profileIdToFavourite);

      var response = new TwilioResponse();

      response.Say(string.Format("{0} has been added as a favourite", profile.FullName));
      response.Redirect(ivrRouteProvider.GetUrlMethod(IVRRoutes.PLAY_MAIN_MENU));

      return response;
    }
  }
}

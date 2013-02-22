using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Mvc;
using Twilio.TwiML;

namespace RefUnitedIVRPlatform.Business.SMSReceiverLogic
{
  public class SMSReceiverLogic : ISMSReceiverLogic
  {
    private string accountSid = "";
    private string authToken = "";
    private string twilioPhoneNumber = "";

    public SMSReceiverLogic(string accountSid, string authToken, string twilioPhoneNumber)
    {
      this.accountSid = accountSid;
      this.authToken = authToken;
      this.twilioPhoneNumber = twilioPhoneNumber;
    }

    public TwilioResponse ResponseToSms(SmsRequest request)
    {
      var response = new TwilioResponse();

      try
      {
        string outboundPhoneNumber = request.From;

        var client = new TwilioRestClient(accountSid, authToken);

        var call = client.InitiateOutboundCall(
          twilioPhoneNumber,
          outboundPhoneNumber,
          "http://refuniteivr.azurewebsites.net/api/IVREntry");

        if (call.RestException == null)
        {
          response.Sms("starting call to " + outboundPhoneNumber);
        }
        else
        {
          response.Sms("failed call to " + outboundPhoneNumber + " " + call.RestException.Message);
        }

        return response;
      }
      catch (Exception ex)
      {
        response.Sms("exception: " + ex.Message);
        return response;
      }
    }
  }
}
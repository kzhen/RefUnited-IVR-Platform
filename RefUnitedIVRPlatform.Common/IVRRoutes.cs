using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Common
{
  public static class IVRRoutes
  {
    public const string PLAY_MAIN_MENU = "PlayMainMenu";
    public const string MAIN_MENU_SELECTION = "MainMenuSelection";
    public const string BROADCASTS_LISTEN_TO_ALL_PUBLIC = "ListenToAllBroadcasts";
    
    public const string BROADCAST_MENU = "ListenToPublicBroadcastMenu";
    public const string BROADCAST_SAVE_PUBLIC_BROADCAST = "SavePublicBroadcast";
    public const string BROADCAST_RECORD = "RecordBroadcast";
    public const string BROADCAST_LISTEN_TO_MATCHED = "ListenToMatchedBroadcasts";
    public const string BROADCAST_RESPONSE_SELECTION = "DetermineResponseSelection";
    public const string BROADCASTS_REPLY_PUBLICLY = "ReplyToBroadcastPrivately";
    public const string BROADCASTS_REPLY_PRIVATELY = "ReplyToBroadcastPublicly";
    public const string BROADCAST_SAVE_PRIVATE_REPLY = "SavePrivateReplyToBroadcast";
    public const string BROADCAST_SAVE_PUBLIC_REPLY = "SavePublicReplyToBroadcast";
    public const string BROADCASTS_PLAY_PUBLIC_REPLY = "PlayPublicReplyToBroadcast";
    public const string BROADCAST_REPLY_RESPONSE_SELECTION = "DetermineResponseToReplyBroadcastMenu";
    public const string BROADCASTS_ADD_REPLIER_AS_FAVOURITE = "AddReplyUserAsAFavourite";
  }
}

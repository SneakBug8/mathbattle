using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace mathbattle.utility
{
    public static class SendToAll
    {
        public static void SendText(PlayerWrapper[] players, string message)
        {
            foreach (var player in players) {
                player.Player.SendMessage(message);
            }
        }
        public static void SendAction(PlayerWrapper[] players, ChatAction action) {
            foreach (var player in players) {
                player.Player.SendAction(action);
            }
        }
    }
}
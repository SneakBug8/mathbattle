﻿using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace mathbattle
{
    public class Player
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public Game Game;
        public ChatId ChatId;
        public Player() {}
        public Player(int id, string name) {
            Id = id;
            Name = name;
        }

        public async Task SendMessage(string text, int delay = 250) {
            SendAction(ChatAction.Typing);
            await Task.Delay(delay);
            await Program.Server.Client.SendTextMessageAsync(ChatId, text);
        }

        public void SendAction(ChatAction action) {
            Program.Server.Client.SendChatActionAsync(ChatId, action);
        }
    }
}
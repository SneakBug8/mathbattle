using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace mathbattle
{
    public class Player
    {
        public int Id;
        public string Name;
        public Game Game;
        public ChatId ChatId;

        public Player(int id, string name, ChatId chatId) {
            Id = id;
            Name = name;
            ChatId = chatId;
        }
        public async Task SendMessage(string text, int delay = 250) {
            await Program.Server.Client.SendChatActionAsync(ChatId, ChatAction.Typing);
            await Task.Delay(delay);
            await Program.Server.Client.SendTextMessageAsync(ChatId, text);
        }
    }
}
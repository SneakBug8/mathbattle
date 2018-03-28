using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace mathbattle
{
    public class Server
    {
        public TelegramBotClient Client;
        public Dictionary<int, Player> Players = new Dictionary<int, Player>();
        public List<Game> Games = new List<Game>();
        public Server(string id)
        {
            Initialize(id);
        }

        async Task Initialize(string id)
        {
            Client = new TelegramBotClient(id);
            var me = await Client.GetMeAsync();
            Console.Title = me.Username;

            Console.WriteLine(me.FirstName + " " + me.LastName + " @" + me.Username);

            Client.OnMessage += OnMessage;

            Client.StartReceiving();
        }

        void OnMessage(object sender, MessageEventArgs args)
        {
            Player curplayer;

            if (!Players.ContainsKey(args.Message.From.Id)) {
                curplayer = new Player();
                curplayer.Id = args.Message.From.Id;
                curplayer.Name = args.Message.From.FirstName + args.Message.From.LastName;
                curplayer.ChatId = args.Message.Chat.Id;
                Players.Add(curplayer.Id, curplayer);

                curplayer.SendMessage("You have been registered in game. Have good time!");
            }
            else {
                curplayer = Players[args.Message.From.Id];
            }

            if (args.Message.Text == null) {
                return;
            }

            if (curplayer.Game != null) {
                curplayer.Game.OnMessage(args.Message);
                return;
            }
            else if (!Program.GameFinder.SearchingPlayers.Contains(curplayer)) {
                Program.GameFinder.AddPlayer(curplayer);
            }
            else {
                curplayer.SendMessage("We're searching a game for you... There are currently " + Players.Count + " online.");
            }
        }
    }
}
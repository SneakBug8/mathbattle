using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        async void Initialize(string id)
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
            Console.WriteLine(args.Message.From.FirstName + ": " + args.Message.Text);
            Player curplayer;

            if (!Players.ContainsKey(args.Message.From.Id))
            {
                curplayer = Program.DataContext.Players
                    .Where(x => x.Id == args.Message.From.Id)
                    .SingleOrDefault(null);

                Console.WriteLine("No such player");
                
                Console.WriteLine(curplayer);

                if (curplayer == null)
                {
                Console.WriteLine("Creating new player");
                    
                    curplayer = new Player(
                        args.Message.From.Id,
                        args.Message.From.FirstName + args.Message.From.LastName
                    );

                    Program.DataContext.Players.Add(curplayer);

                    var count = Program.DataContext.SaveChanges();
                    Console.WriteLine("{0} records saved to database", count);

                    curplayer.SendMessage("You have been registered in game. Have good time!");
                }

                curplayer.ChatId = args.Message.Chat.Id;

                Players.Add(curplayer.Id, curplayer);
            }
            else
            {
                Console.WriteLine("Has such player");
                
                curplayer = Players[args.Message.From.Id];
            }

            if (args.Message.Text == null)
            {
                return;
            }

            if (curplayer.Game != null)
            {
                curplayer.Game.OnMessage(args.Message);
                return;
            }
            else if (!Program.GameFinder.Contains(curplayer))
            {
                Program.GameFinder.AddPlayer(curplayer);
            }
            else
            {
                curplayer.SendMessage("We're searching a game for you... There are currently " + Players.Count + " online.");
            }
        }
    }
}
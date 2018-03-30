using System;
using System.Threading.Tasks;
using Telegram.Bot;
using mathbattle.config;
using mathbattle.database;

namespace mathbattle
{
    class Program
    {
        
        public static GameFinder GameFinder;
        public static Server Server;

        public static DataContext DataContext;
        static void Main(string[] args)
        {
            DataContext = new DataContext();
            Server = new Server(GlobalConfig.Token);
            GameFinder = new GameFinder();

            Loop();

            while (true) {}
        }

        static async void Loop() {
            while (true)
            {
                await Task.Delay(GlobalConfig.LoopDelay);
                GameFinder.CheckPlayers();
            }
        }
    }
}
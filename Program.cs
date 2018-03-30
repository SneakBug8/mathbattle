using System;
using System.Threading.Tasks;
using Telegram.Bot;
using mathbattle.config;

namespace mathbattle
{
    class Program
    {
        
        public static GameFinder GameFinder;
        public static Server Server;
        static void Main(string[] args)
        {
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
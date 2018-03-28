using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace mathbattle
{
    class Program
    {
        
        public static GameFinder GameFinder;
        public static Server Server;
        static void Main(string[] args)
        {
            Server = new Server("386013421:AAHo6qF6ccQ8Lxpb5uf2PO5KOE-brSz2voU");
            GameFinder = new GameFinder();

            Loop();

            while (true) {}
        }

        static async void Loop() {
            while (true)
            {
                await Task.Delay(1000);
                GameFinder.CheckPlayers();
            }
        }
    }
}
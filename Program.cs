using System.Threading.Tasks;
using mathbattle.config;
using LiteDB;
using System.IO;
using System.Text.RegularExpressions;
using mathbattle.SearchingSystem;

namespace mathbattle
{
    class Program
    {

        public static GameSearcher GameFinder;
        public static Server Server;
        public static LiteDatabase Database;
        static void Main(string[] args)
        {
            var dbpath = GetApplicationRoot() + @"\MyData.db";
            Database = new LiteDatabase(dbpath);
            Server = new Server(GlobalConfig.Token);
            GameFinder = new GameSearcher();

            Loop();

            while (true) { }
        }

        static async void Loop()
        {
            while (true)
            {
                await Task.Delay(GlobalConfig.LoopDelay);
                GameFinder.CheckPlayers();
            }
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
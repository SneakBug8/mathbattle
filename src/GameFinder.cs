using System.Collections.Generic;

namespace mathbattle
{
    public class GameFinder
    {
        public List<Player> SearchingPlayers = new List<Player>();

        public void AddPlayer(Player player) {
            SearchingPlayers.Add(player);

            player.SendMessage("You have been added to search queue. Waiting for the game.");
        }
        public void CheckPlayers() {
            if (SearchingPlayers.Count >= 2) {
                var game = new Game(SearchingPlayers.ToArray());
                SearchingPlayers = new List<Player>();
                foreach (var gameplayer in game.GamePlayers) {
                    gameplayer.Player.Game = game;
                }
                Program.Server.Games.Add(game);
            }
        }
    }
}
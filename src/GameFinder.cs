using System.Collections.Generic;

namespace mathbattle
{
    public class GameFinder
    {
        public int TimeTillRemoveFromSearching = 120;
        public List<SearchingPlayer> SearchingPlayers = new List<SearchingPlayer>();

        public void AddPlayer(Player player) {
            SearchingPlayers.Add(new SearchingPlayer(player, TimeTillRemoveFromSearching));

            player.SendMessage("You have been added to search queue. Waiting for the game.");
        }
        public void CheckPlayers() {
            if (SearchingPlayers.Count >= 2) {
                var game = new Game(GetPlayersList(SearchingPlayers));
                SearchingPlayers = new List<SearchingPlayer>();
                foreach (var gameplayer in game.GamePlayers) {
                    gameplayer.Player.Game = game;
                }
                Program.Server.Games.Add(game);
            }

            CheckOldPlayers();
        }

        void CheckOldPlayers() {
            foreach (var searchingplayer in SearchingPlayers) {
                searchingplayer.TimeTillRemoveFromSearching--;

                if (searchingplayer.TimeTillRemoveFromSearching <= 0) {
                    searchingplayer.Player.SendMessage("You have been deleted from search due to inactivity");
                    SearchingPlayers[SearchingPlayers.IndexOf(searchingplayer)] = null;
                }
            }

            while (SearchingPlayers.Contains(null)) {
                SearchingPlayers.Remove(null);
            }
        }

        public Player[] GetPlayersList(List<SearchingPlayer> searchingPlayers) {
            var players = new Player[searchingPlayers.Count];

            for (int i = 0; i < searchingPlayers.Count; i++) {
                players[i] = searchingPlayers[i].Player;
            }

            return players;
        }

        public bool Contains(Player player) {
            foreach (var searchingplayer in SearchingPlayers) {
                if (searchingplayer.Player == player) {
                    return true;
                }
            }

            return false;
        }
    }

    public class SearchingPlayer {
        public Player Player;
        public int TimeTillRemoveFromSearching;

        public SearchingPlayer(Player player, int timeTillRemoveFromSearching) {
            Player = player;
            TimeTillRemoveFromSearching = timeTillRemoveFromSearching;
        }
    }
}
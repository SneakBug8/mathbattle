using System.Collections.Generic;
using mathbattle.utility;
using mathbattle.config;

namespace mathbattle
{
    public class GameFinder
    {
        public List<SearchingPlayer> SearchingPlayers = new List<SearchingPlayer>();

        public void AddPlayer(Player player)
        {
            SearchingPlayers.Add(new SearchingPlayer(player, SearcherConfig.TimeTillDeleteFromSearch));

            player.SendMessage("You have been added to search queue. Waiting for the game.");
        }
        public void CheckPlayers()
        {
            if (SearchingPlayers.Count >= SearcherConfig.MinimumPlayersForGameCreation)
            {
                DelayedTask.DelayTask(new List<DelayedTask>() {
                new DelayedTask(() => StartGame(), 60),
                new DelayedTask(() => SendToAll.SendText(SearchingPlayers.ToArray(), "Game will start in 60 seconds."), 1),                
                new DelayedTask(() => SendToAll.SendText(SearchingPlayers.ToArray(), "Game will start in 30 seconds."), 30),
                new DelayedTask(() => SendToAll.SendText(SearchingPlayers.ToArray(), "Game will start in 15 seconds."), 45),
                new DelayedTask(() => SendToAll.SendText(SearchingPlayers.ToArray(), "Game will start in 5 seconds."), 55)
                });
            }

            CheckOldPlayers();
        }

        void CheckOldPlayers()
        {
            for (int i = 0; i < SearchingPlayers.Count; i++)
            {
                SearchingPlayers[i].TimeTillRemoveFromSearching--;

                if (SearchingPlayers[i].TimeTillRemoveFromSearching <= 0)
                {
                    SearchingPlayers[i].Player.SendMessage("You have been deleted from search due to inactivity");
                    SearchingPlayers[i] = null;
                }
            }

            while (SearchingPlayers.Contains(null))
            {
                SearchingPlayers.Remove(null);
            }
        }

        void StartGame()
        {
            if (SearchingPlayers.Count < 2)
            {
                return;
            }

            // Create new game and assign there all players
            var game = new Game(GetPlayersList(SearchingPlayers));

            // Empty Searching Players
            SearchingPlayers = new List<SearchingPlayer>();

            foreach (var gameplayer in game.GamePlayers)
            {
                gameplayer.Player.Game = game;
            }
            Program.Server.Games.Add(game);
        }

        public Player[] GetPlayersList(List<SearchingPlayer> searchingPlayers)
        {
            var players = new Player[searchingPlayers.Count];

            for (int i = 0; i < searchingPlayers.Count; i++)
            {
                players[i] = searchingPlayers[i].Player;
            }

            return players;
        }

        public bool Contains(Player player)
        {
            foreach (var searchingplayer in SearchingPlayers)
            {
                if (searchingplayer.Player == player)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class SearchingPlayer : PlayerWrapper
    {
        public int TimeTillRemoveFromSearching;

        public SearchingPlayer(Player player, int timeTillRemoveFromSearching)
        {
            Player = player;
            TimeTillRemoveFromSearching = timeTillRemoveFromSearching;
        }
    }
}
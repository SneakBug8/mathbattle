using System.Collections.Generic;
using mathbattle.utility;

namespace mathbattle.GameSearcher
{
    public class SearchingParty
    {
        public List<SearchingPlayer> Players;
        public TaskDelayer Delayer;

        public SearchingParty(List<SearchingPlayer> players)
        {
            Players = players;

            if (players.Count < 2)
            {
                throw new System.Exception("Not enought players for game creation");
            }

            StartDelay();
        }

        void StartDelay()
        {
            Delayer = DelayedTask.DelayTask(new List<DelayedTask>() {
                new DelayedTask(() => StartGame(), 60),
                new DelayedTask(() => SendToAll.SendText(Players.ToArray(), "Game will start in 60 seconds."), 0),
                new DelayedTask(() => SendToAll.SendText(Players.ToArray(), "Game will start in 30 seconds."), 30),
                new DelayedTask(() => SendToAll.SendText(Players.ToArray(), "Game will start in 15 seconds."), 45),
                new DelayedTask(() => SendToAll.SendText(Players.ToArray(), "Game will start in 5 seconds."), 55)
                });
        }

        void StartGame()
        {
            // Create new game and assign there all players
            var game = new Game(GetPlayersList(Players));

            foreach (var gameplayer in game.GamePlayers)
            {
                gameplayer.Player.Game = game;
            }

            Program.Server.Games.Add(game);
        }

        public void AddPlayer(SearchingPlayer player)
        {
            Players.Add(player);
            player.Player.SendMessage(
                string.Format("Game will start in {0} seconds.", 60 - Delayer.CurrentTimer)
                );
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
    }
}
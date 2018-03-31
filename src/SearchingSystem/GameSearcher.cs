using System.Collections.Generic;
using mathbattle.utility;
using mathbattle.config;

namespace mathbattle.SearchingSystem
{
    public class GameSearcher
    {
        public List<SearchingPlayer> SearchingPlayers = new List<SearchingPlayer>();

        public List<SearchingParty> SearchingParties = new List<SearchingParty>();

        public void AddPlayer(Player player)
        {
            SearchingPlayers.Add(new SearchingPlayer(player, SearcherConfig.TimeTillDeleteFromSearch));

            player.SendMessage("You have been added to search queue. Please, wait.");
        }
        public void CheckPlayers()
        {
            if (SearchingPlayers.Count > 0 && SearchingParties.Count > 0) {
                SearchingParties[0].AddPlayer(SearchingPlayers[0]);
                SearchingPlayers.RemoveAt(0);
            }
            else if (SearchingPlayers.Count >= SearcherConfig.MinimumPlayersForGameCreation)
            {
                var party = new SearchingParty(SearchingPlayers);
                SearchingPlayers = new List<SearchingPlayer>();
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

    
}
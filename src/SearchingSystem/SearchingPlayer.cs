using mathbattle;
using mathbattle.utility;

namespace mathbattle.SearchingSystem {
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
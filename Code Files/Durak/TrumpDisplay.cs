namespace Durak
{
    /// <summary>
    /// Stores and provides the current trump suit for the game.
    /// This will later be used by the UI to display the trump suit
    /// during gameplay.
    /// </summary>
    public class TrumpDisplay
    {
        private string trumpSuit = "None";

        public void SetTrumpSuit(string suit)
        {
            trumpSuit = suit;
        }

        public string GetTrumpSuit()
        {
            return trumpSuit;
        }

        public bool HasTrump()
        {
            return trumpSuit != "None";
        }
    }
}
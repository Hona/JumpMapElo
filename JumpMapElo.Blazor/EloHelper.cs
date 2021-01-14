using System;

namespace JumpMapElo.Blazor
{
    public enum Outcome
    {
        Win = 1, 
        Loss = 0
    }
    
    public static class EloHelper
    {
        public static double ExpectationToWin(int playerOneRating, int playerTwoRating)
        {
            return 1 / (1 + Math.Pow(10, (playerTwoRating - playerOneRating) / 400.0));
        }

        public static void CalculateElo(ref int playerOneRating, ref int playerTwoRating, Outcome outcome)
        {
            const int eloK = 13;
            
            var delta = (int)(eloK * ((int)outcome - ExpectationToWin(playerOneRating, playerTwoRating)));

            playerOneRating += delta;
            playerTwoRating -= delta;
        }
    }
}
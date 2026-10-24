using System.Collections.Generic;

namespace TicTacToe
{

    public static class Rules
    {
        // Все 8 выигрышных линий
        static readonly int[][] WinLines =
        {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8},   // строки
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8},   // столбцы
            new[] {0, 4, 8}, new[] {2, 4, 6}                     // диагонали
        };


        public static List<int> GetValidMoves(GameState s)
        {
            var moves = new List<int>();
            if (GetWinner(s) != Cell.Empty) return moves;

            for (int i = 0; i < 9; i++)
                if (s.Board[i] == Cell.Empty) moves.Add(i);

            return moves;
        }
        public static GameState ApplyMove(GameState s, int move)
        {
            var next = s.Clone();
            next.Board[move] = s.CurrentPlayer;
            next.CurrentPlayer = s.CurrentPlayer == Cell.Human ? Cell.AI : Cell.Human;
            return next;
        }
        public static Cell GetWinner(GameState s)
        {
            foreach (var line in WinLines)
            {
                Cell c = s.Board[line[0]];
                if (c != Cell.Empty && c == s.Board[line[1]] && c == s.Board[line[2]])
                    return c;
            }
            return Cell.Empty;
        }
        public static bool IsTerminal(GameState s)
        {
            if (GetWinner(s) != Cell.Empty) return true;
            foreach (var c in s.Board)
                if (c == Cell.Empty) return false;
            return true;
        }

        public static int Evaluate(GameState s)
        {
            switch (GetWinner(s))
            {
                case Cell.AI: return 10;
                case Cell.Human: return -10;
                default: return 0;
            }
        }
    }
}
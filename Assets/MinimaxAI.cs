using System;
using System.Diagnostics;
using TicTacToe;

/// <summary>
/// ИИ на основе алгоритма Minimax с полным перебором дерева игры.
/// ИИ (O) — максимизирующий игрок, человек (X) — минимизирующий.
/// </summary>
public class MinimaxAI
{
    public int NodesVisited { get; private set; }
    public long ElapsedMs { get; private set; }

    /// <summary>Выбирает лучший ход для ИИ в текущей позиции.</summary>
    public int GetBestMove(GameState state)
    {
        NodesVisited = 0;
        var sw = Stopwatch.StartNew();

        int bestMove = -1;
        int bestScore = int.MinValue;

        foreach (int move in Rules.GetValidMoves(state))
        {
            // После хода ИИ очередь человека, поэтому isMaximizing = false
            int score = Minimax(Rules.ApplyMove(state, move), 1, false);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        sw.Stop();
        ElapsedMs = sw.ElapsedMilliseconds;
        return bestMove;
    }

    /// <summary>
    /// Рекурсивный Minimax.
    /// depth — текущая глубина узла (для статистики; ограничения глубины здесь не нужно).
    /// isMaximizing — true, если ходит ИИ; false, если ходит человек.
    /// </summary>
    public int Minimax(GameState state, int depth, bool isMaximizing)
    {
        NodesVisited++;

        if (Rules.IsTerminal(state))
            return Rules.Evaluate(state);

        var moves = Rules.GetValidMoves(state);

        if (isMaximizing)
        {
            int best = int.MinValue;
            foreach (int move in moves)
            {
                int score = Minimax(Rules.ApplyMove(state, move), depth + 1, false);
                best = Math.Max(best, score);
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            foreach (int move in moves)
            {
                int score = Minimax(Rules.ApplyMove(state, move), depth + 1, true);
                best = Math.Min(best, score);
            }
            return best;
        }
    }
}
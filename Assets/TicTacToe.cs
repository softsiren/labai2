namespace TicTacToe
{
    public enum Cell { Empty, Human, AI }


    public class GameState
    {
        public Cell[] Board = new Cell[9];
        public Cell CurrentPlayer = Cell.Human;

        public GameState Clone()
        {
            return new GameState
            {
                Board = (Cell[])Board.Clone(),
                CurrentPlayer = CurrentPlayer
            };
        }
    }
}
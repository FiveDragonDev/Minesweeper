namespace Minesweeper.Game.World
{
    public interface IWorldEvents
    {
        event Action<WorldPos, byte>? CellRevealed;
        event Action<WorldPos, bool>? CellFlagToggled;
        event Action<WorldPos>? MineExploded;
        event Action<WorldPos>? RevealTriggered;
    }
    public sealed partial class World : IWorldEvents
    {
        public event Action<WorldPos, byte>? CellRevealed;
        public event Action<WorldPos, bool>? CellFlagToggled;
        public event Action<WorldPos>? MineExploded;
        public event Action<WorldPos>? RevealTriggered;
    }
}

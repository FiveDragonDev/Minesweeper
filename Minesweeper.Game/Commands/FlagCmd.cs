using Minesweeper.Game.World;

namespace Minesweeper.Game.Commands
{
    public readonly record struct FlagCmd(WorldPos Position) : ICommand
    {
        public void Apply(IWorldController world) => world.ToggleFlag(Position);
    }
}

using Minesweeper.Game.World;

namespace Minesweeper.Game.Commands
{
    public readonly record struct RevealCmd(WorldPos Position) : ICommand
    {
        public void Apply(IWorldController world) => world.Reveal(Position);
    }
}

using Minesweeper.Game.World;

namespace Minesweeper.Game.Commands
{
    public interface ICommand
    {
        void Apply(IWorldController world);
    }
}

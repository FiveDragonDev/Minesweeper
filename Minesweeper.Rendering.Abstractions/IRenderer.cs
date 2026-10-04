using Minesweeper.Game.World;

namespace Minesweeper.Rendering.Abstractions
{
    public interface IRenderer
    {
        void Render(IWorldView worldView);
    }
}

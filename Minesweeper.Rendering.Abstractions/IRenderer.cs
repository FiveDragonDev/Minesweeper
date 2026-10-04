using Minesweeper.Game;

namespace Minesweeper.Rendering.Abstractions
{
    public interface IRenderer
    {
        void Render(IGameView gameView);
    }
}

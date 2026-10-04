using Minesweeper.Game.Commands;
using Minesweeper.Game.World;
using Minesweeper.Input.Abstractions;

namespace Minesweeper.App
{
    public sealed class InputActionMapper
    {
        public ICommand? Map(InputAction action, WorldPos at) => action switch
        {
            InputAction.Reveal => new RevealCmd(at),
            InputAction.Flag => new FlagCmd(at),
            _ => null
        };
    }
}

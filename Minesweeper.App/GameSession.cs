using Minesweeper.Game.Commands;
using Minesweeper.Game.Player;
using Minesweeper.Input.Abstractions;

namespace Minesweeper.App
{
    public sealed class GameSession(IInputSource input, ICursor cursor,
        CursorController cursorController, InputActionMapper mapper, CommandProcessor commands)
    {
        private readonly IInputSource _input = input;
        private readonly ICursor _cursor = cursor;
        private readonly CursorController _cursorController = cursorController;
        private readonly InputActionMapper _mapper = mapper;
        private readonly CommandProcessor _commands = commands;

        public void Tick(float dt, int commandBudget)
        {
            _input.Update();

            _cursorController.Update(_input.GetMoveAxis(), dt);

            foreach (var action in _input.PollActions())
            {
                var cmd = _mapper.Map(action, _cursor.Cell);
                if (cmd is not null) _commands.Enqueue(cmd);
            }

            _commands.Process(commandBudget);
        }
    }
}

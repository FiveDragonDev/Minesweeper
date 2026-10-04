using Minesweeper.Game;
using Minesweeper.Input.Abstractions;

namespace Minesweeper.App
{
    public sealed class GameSession(IInputSource input, GameState state, InputActionMapper mapper)
    {
        private readonly IInputSource _input = input;
        private readonly GameState _state = state;
        private readonly InputActionMapper _mapper = mapper;

        public void Tick(float dt, int commandBudget)
        {
            var axis = _input.GetMoveAxis();
            var commands = _input.PollActions().Select(action => _mapper.Map(action, _state.Cursor.Cell)).Where(cmd => cmd is not null).Select(c => c!).ToList();

            _state.Update(axis, commands, dt, commandBudget);
        }
    }
}

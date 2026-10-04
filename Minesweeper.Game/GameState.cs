using Minesweeper.Game.Chunks;
using Minesweeper.Game.Commands;
using Minesweeper.Game.Player;
using Minesweeper.Game.World;
using System.Numerics;

namespace Minesweeper.Game
{
    public interface IGameView
    {
        IWorldView World { get; }
        ICursor Cursor { get; }
    }

    public sealed class GameState : IGameView
    {
        IWorldView IGameView.World => World;
        ICursor IGameView.Cursor => Cursor;

        public World.World World { get; }
        public Cursor Cursor { get; }

        private readonly CursorController _cursorController;
        private readonly CommandProcessor _commands;

        public GameState(ulong seed, int minePercent)
        {
            World = new(seed, minePercent);
            Cursor = new(new(ChunkTerrain.Size >> 1, ChunkTerrain.Size >> 1));
            _cursorController = new(Cursor) { Speed = 8 };
            _commands = new(World);
        }

        public void Update(Vector2 axis, IEnumerable<ICommand> commands, float dt, int commandBudget)
        {
            _cursorController.Update(axis, dt);

            foreach (var cmd in commands) _commands.Enqueue(cmd);
            _commands.Process(commandBudget);
        }

        public void TickWorld(int cellBudget) => World.Update(cellBudget);

        public void UnloadFarChunks(int radius) => World.UnloadFarChunks(ChunkCoords.ToChunk(Cursor.Cell), radius);
    }
}
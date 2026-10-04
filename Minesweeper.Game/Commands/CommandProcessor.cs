using Minesweeper.Game.World;

namespace Minesweeper.Game.Commands
{
    public sealed class CommandProcessor(IWorldController world)
    {
        private readonly Queue<ICommand> _queue = [];
        private readonly IWorldController _world = world;

        public void Enqueue(ICommand cmd) => _queue.Enqueue(cmd);

        public void Process(int budget)
        {
            int processed = 0;
            while (processed < budget && _queue.TryDequeue(out var cmd))
            {
                cmd.Apply(_world);
                processed++;
            }
        }
    }
}
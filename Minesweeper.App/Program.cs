using Minesweeper.Client.Sfml;
using Minesweeper.Game;
using SFML.System;

namespace Minesweeper.App
{
    internal sealed class Program
    {
        private static void Main()
        {
            const ulong seed = 0;
            const int minePercent = 15;
            const float updateInterval = 1f / 20;
            const int cellBudget = 128;
            const int commandsBudget = 16;
            const int unloadRadius = 4;

            GameState state = new(seed, minePercent);

            using SfmlWindow window = new(800, 600, "Minesweeper");

            SfmlInputSource input = new(window);
            InputActionMapper mapper = new();

            GameSession session = new(input, state, mapper);

            SfmlRenderer renderer = new(window);

            bool running = true;
            Console.CancelKeyPress += (s, e) => running = false;
            window.Closed += () => running = false;

            Clock clock = new();

            float accumulator = 0;
            float dt = 0;
            while (running)
            {
                dt = clock.Restart().AsSeconds();

                window.PollEvents();
                input.Update(dt);

                accumulator += dt;
                while (accumulator >= updateInterval)
                {
                    session.Tick(updateInterval, commandsBudget);
                    state.TickWorld(cellBudget);
                    state.UnloadFarChunks(unloadRadius);
                    accumulator -= updateInterval;
                }

                renderer.Render(state);
            }
        }
    }
}

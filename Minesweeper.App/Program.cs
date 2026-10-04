using Minesweeper.Client.Sfml;
using Minesweeper.Game.Chunks;
using Minesweeper.Game.Commands;
using Minesweeper.Game.Player;
using Minesweeper.Game.World;
using SFML.System;

namespace Minesweeper.App
{
    internal sealed class Program
    {
        private static void Main()
        {
            const ulong seed = 0;
            const int minePercent = 15;

            World world = new(seed, minePercent);

            Cursor cursor = new(new(ChunkTerrain.Size >> 1, ChunkTerrain.Size >> 1));
            CursorController cursorController = new(cursor)
            {
                Speed = 8
            };

            CommandProcessor commands = new(world);

            using SfmlWindow window = new(800, 600, "Minesweeper");

            SfmlInputSource inputSource = new(window);
            InputActionMapper mapper = new();

            SfmlRenderer renderer = new(window);

            GameSession session = new(inputSource, cursor, cursorController, mapper, commands);

            bool running = true;
            window.Closed += () => running = false;

            Clock clock = new();
            const float updateInterval = 1f / 20;

            float accumulator = 0;
            float dt = 0;

            while (running)
            {
                dt = clock.Restart().AsSeconds();

                window.PollEvents();

                session.Tick(dt, 16);

                renderer.SetPosition(cursor.Position, cursor.Cell);

                accumulator += dt;
                while (accumulator >= updateInterval)
                {
                    world.Update(128);
                    world.UnloadFarChunks(ChunkCoords.ToChunk(cursor.Cell), 4);
                    accumulator -= updateInterval;
                }

                renderer.Render(world);
            }
        }
    }
}

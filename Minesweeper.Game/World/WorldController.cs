using Minesweeper.Game.Chunks;

namespace Minesweeper.Game.World
{
    public interface IWorldController
    {
        void ToggleFlag(WorldPos p);
        RevealOutcome Reveal(WorldPos p);
    }
    public sealed partial class World : IWorldController
    {
        public void ToggleFlag(WorldPos p)
        {
            if (IsRevealed(p)) return;
            var f = GetOrCreateChunkAt(ChunkCoords.ToChunk(p)).ToggleFlag(ChunkCoords.ToLocal(p));
            CellFlagToggled?.Invoke(p, f);
        }

        public RevealOutcome Reveal(WorldPos p)
        {
            if (IsRevealed(p)) return RevealAround(p);
            if (IsFlagged(p)) return RevealOutcome.Halted;

            if (IsMine(p))
            {
                GetOrCreateChunkAt(ChunkCoords.ToChunk(p)).Reveal(ChunkCoords.ToLocal(p), 0);
                MineExploded?.Invoke(p);
                return RevealOutcome.HitMine;
            }

            if (!TryEnqueue(p)) return RevealOutcome.Ok;

            RevealTriggered?.Invoke(p);

            return RevealOutcome.Pending;
        }

        private RevealOutcome RevealAround(WorldPos p)
        {
            if (IsMine(p)) return RevealOutcome.Halted;

            byte flags = 0, mines = 0;
            for (int i = 0; i < Neighbors.Length; i++)
            {
                WorldPos np = new(p.X + Neighbors[i].DX, p.Y + Neighbors[i].DY);
                if (IsMine(np))
                {
                    mines++;
                    if (IsRevealed(np)) flags++;
                }
                if (IsFlagged(np)) flags++;
            }

            if (mines == 0 || flags != mines) return RevealOutcome.Halted;

            bool any = false;
            for (int i = 0; i < Neighbors.Length; i++)
            {
                WorldPos np = new(p.X + Neighbors[i].DX, p.Y + Neighbors[i].DY);

                if (IsFlagged(np)) continue;

                if (IsMine(np))
                {
                    GetOrCreateChunkAt(ChunkCoords.ToChunk(np)).Reveal(ChunkCoords.ToLocal(np), 0);
                    MineExploded?.Invoke(np);
                    return RevealOutcome.HitMine;
                }
                else if (TryEnqueue(np)) any = true;
            }

            return any ? RevealOutcome.Pending : RevealOutcome.Ok;
        }
    }
}

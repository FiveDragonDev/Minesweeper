using System.Text.Json;

namespace Minesweeper.Game.Save
{
    public static class SaveSystem
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
        };

        public static void Save(GameState state, string path)
        {
            var data = state.ToSave();
            File.WriteAllText(path, JsonSerializer.Serialize(data, Options));
        }

        public static GameState Load(string path)
        {
            var data = JsonSerializer.Deserialize<SaveData>(File.ReadAllText(path)) ?? throw new InvalidDataException("Empty save");
            if (data.Version != 1) throw new InvalidDataException($"Unsupported version {data.Version}");
            return GameState.FromSave(data);
        }
    }
}

using MineNET.NBT.Tags;
using WorldConverter.Format.Data;

namespace WorldConverter.Format
{
    public interface ILevelProvider
    {
        string FolderPath { get; }

        ILevelData LevelData { get; }

        int Dimension { get; }

        bool IsValid(string path);

        void Convert(string levelPath, string savePath, int dimension);

        Chunk DeserializeChunk(CompoundTag tag);

        CompoundTag SerializeChunk(Chunk chunk);
    }
}

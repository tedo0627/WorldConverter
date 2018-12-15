using MineNET.NBT.Tags;

namespace WorldConverter.Format
{
    public interface ILevelProvider
    {
        string FolderPath { get; }

        CompoundTag LevelData { get; }
        
        bool IsValid(string path);

        void SetWorldName(string name);

        void ConvertAsync(string loadPath, string savePath);
    }
}

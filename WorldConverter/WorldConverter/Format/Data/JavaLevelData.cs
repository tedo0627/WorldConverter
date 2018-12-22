using System.IO;
using MineNET.NBT.Data;
using MineNET.NBT.IO;
using MineNET.NBT.Tags;

namespace WorldConverter.Format.Data
{
    public class JavaLevelData : ILevelData
    {
        public string FolderPath { get; }

        public CompoundTag Data { get; private set; }

        public JavaLevelData(string path)
        {
            this.FolderPath = Path.Combine(path, "level.dat");
        }

        public void Load()
        {
            CompoundTag tag = NBTIO.ReadGZIPFile(this.FolderPath, NBTEndian.BIG_ENDIAN);
            this.Data = tag.GetCompound("").GetCompound("Data");
        }

        public void Save()
        {
            CompoundTag tag = new CompoundTag();
            tag.PutCompound("Data", this.Data);
            tag = new CompoundTag().PutCompound("", tag);
            NBTIO.WriteGZIPFile(this.FolderPath, tag, NBTEndian.BIG_ENDIAN);
        }

        public void SetWorldName(string name)
        {
            this.Data.PutString("LevelName", name);
        }

        public void SetWorldSpawn(int dimension)
        {
            switch (dimension)
            {
                case 1:
                    this.Data.PutInt("SpawnX", this.Data.GetInt("SpawnX") / 8);
                    this.Data.PutInt("SpawnZ", this.Data.GetInt("SpawnZ") / 8);
                    break;
                case 2:
                    this.Data.PutInt("SpawnX", 100);
                    this.Data.PutInt("SpawnY", 49);
                    this.Data.PutInt("SpawnZ", 0);
                    break;
            }
        }
    }
}

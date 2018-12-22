using System.IO;
using MineNET.NBT.Tags;
using WorldConverter.Format.Data;
using WorldConverter.Format.Region;

namespace WorldConverter.Format
{
    public abstract class BaseLevelProvider : ILevelProvider
    {
        public string FolderPath { get; protected set; }
        
        public ILevelData LevelData { get; protected set; }

        public int Dimension { get; protected set; }

        public abstract bool IsValid(string path);

        public void Convert(string levelPath, string savePath, int dimension)
        {
            MainForm form = MainForm.Form;

            this.Dimension = dimension;
            string regionPath = this.GetRegionPath(dimension, levelPath);

            levelPath = Path.Combine(levelPath, "level.dat");
            if (!File.Exists(levelPath))
            {
                form.Log($"ファイル {levelPath} が見つかりませんでした");
                form.IsExcecuting = false;
                return;
            }
            if (!Directory.Exists(regionPath))
            {
                form.Log($"パス {regionPath} が見つかりませんでした");
                form.IsExcecuting = false;
                return;
            }
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            File.Copy(levelPath, Path.Combine(savePath, "level.dat"), true);
            this.LoadLevelData(savePath);
            this.LevelData.Load();
            this.LevelData.SetWorldName(form.GetWorldName());
            this.LevelData.SetWorldSpawn(dimension);
            this.LevelData.Save();


            string[] files = Directory.GetFiles(regionPath);

            form.SetProgressValue(0);
            form.SetMaxProgress(files.Length * 1024);

            if (!Directory.Exists(Path.Combine(savePath, "region")))
            {
                Directory.CreateDirectory(Path.Combine(savePath, "region"));
            }
            
            for (int i = 0; i < files.Length; ++i)
            {
                string name = Path.GetFileName(files[i]);
                string copyFile = Path.Combine(savePath, "region", name);
                File.Copy(files[i], copyFile, true);
                RegionLoader b = new RegionLoader(this, copyFile);
                b.Convert();
            }
        }

        public abstract void LoadLevelData(string path);

        public abstract Chunk DeserializeChunk(CompoundTag tag);

        public abstract CompoundTag SerializeChunk(Chunk chunk);
        
        public string GetRegionPath(int dimension, string path)
        {
            switch (dimension)
            {
                case 0:
                    return Path.Combine(path, "region");

                case 1:
                    return Path.Combine(path, "DIM-1", "region");

                case 2:
                    return Path.Combine(path, "DIM1", "region");
            }
            throw new InvalidDataException();
        }
    }
}

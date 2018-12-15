using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MineNET.NBT.Data;
using MineNET.NBT.IO;
using MineNET.NBT.Tags;

namespace WorldConverter.Format.Anvils
{
    public class Anvil : ILevelProvider
    {
        public string FolderPath { get; private set; }

        public CompoundTag LevelData { get; private set; }

        public List<BaseRegionLoader> Loaders { get; private set; } = new List<BaseRegionLoader>();
        
        public bool IsValid(string path)
        {
            if (!File.Exists($"{path}\\level.dat") || !File.GetAttributes($"{path}\\region\\").HasFlag(FileAttributes.Directory))
            {
                return false;
            }
            string[] files = Directory.GetFiles($"{path}\\region\\");
            for (int i = 0; i < files.Length; ++i) {
                if (!Path.GetFileName(files[i]).EndsWith(".mca"))
                {
                    return false;
                }
            }
            return true;
        }

        public async void ConvertAsync(string loadPath, string savePath)
        {
            MainForm form = MainForm.Form;
            form.SetProgressValue(0);

            Directory.CreateDirectory(savePath);
            Directory.CreateDirectory($"{savePath}\\region");
            this.FolderPath = loadPath;


            CompoundTag levelData = NBTIO.ReadGZIPFile($"{loadPath}\\level.dat", NBTEndian.BIG_ENDIAN);
            this.LevelData = levelData.GetCompound("Data");

            this.SetWorldName(MainForm.Form.GetWorldName());
            
            levelData.PutCompound("Data", this.LevelData);
            NBTIO.WriteGZIPFile($"{savePath}\\level.dat", levelData, NBTEndian.BIG_ENDIAN);

            string[] files = Directory.GetFiles($"{loadPath}\\region\\");

            form.SetMaxProgress(files.Length* 3);

            for (int i = 0; i < files.Length; ++i)
            {
                BaseRegionLoader b = new BaseRegionLoader(this, files[i]);
                form.Log($"ファイル {Path.GetFileName(files[i])} を読み込み中...");
                await Task.Run(() => b.LoadChunks());
                form.AddProgressValue(1);
                await Task.Run(() => b.Convert());
                form.AddProgressValue(1);
                await Task.Run(() => b.Save($"{savePath}\\region"));
                form.AddProgressValue(1);
            }

            form.Log("変換が完了しました");
            form.IsExcecuting = false;
        }

        public void SetWorldName(string name)
        {
            this.LevelData.PutString("LevelName", name);
        }
    }
}

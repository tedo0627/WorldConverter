using System.IO;
using WorldConverter.Format.Data;

namespace WorldConverter.Format.Region
{
    public abstract class RegionLevelProvider : BaseLevelProvider
    {
        public abstract string FileExtension { get; }

        public override bool IsValid(string path)
        {
            if (!File.Exists($"{path}\\level.dat") || !File.GetAttributes($"{path}\\region\\").HasFlag(FileAttributes.Directory))
            {
                return false;
            }
            string[] files = Directory.GetFiles($"{path}\\region\\");
            for (int i = 0; i < files.Length; ++i)
            {
                if (!Path.GetFileName(files[i]).EndsWith(this.FileExtension))
                {
                    return false;
                }
            }
            return true;
        }

        public override void LoadLevelData(string path)
        {
            this.LevelData = new JavaLevelData(path);
        }
    }
}

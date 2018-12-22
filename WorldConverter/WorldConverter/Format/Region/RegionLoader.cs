using System;
using System.IO;
using System.Threading.Tasks;
using MineNET.NBT.Data;
using MineNET.NBT.IO;
using MineNET.NBT.Tags;

namespace WorldConverter.Format.Region
{
    public class RegionLoader
    {
        public int X { get; }
        public int Z { get; }

        public ILevelProvider Provider { get; }

        public string FilePath { get; }

        public RegionLoader(ILevelProvider provider, string path)
        {
            this.Provider = provider;
            this.FilePath = path;

            string fileName = Path.GetFileName(path);
            string[] xz = fileName.Split('.');
            this.X = int.Parse(xz[1]);
            this.Z = int.Parse(xz[2]);
        }

        public async void Convert()
        {
            MainForm form = MainForm.Form;
            for (int x = 0; x < 32; ++x)
            {
                for (int z = 0; z < 32; ++z)
                {
                    try
                    {
                        Chunk chunk = await Task.Run(() => this.LoadChunk(x, z));
                        if (chunk != null)
                        {
                            await Task.Run(() => chunk.Convert());
                            await Task.Run(() => this.Save(chunk));
                        }
                        form.AddProgressValue(1);
                    }
                    catch
                    {
                        form.IsErrorCancelled = true;
                        return;
                    }
                }
            }
        }

        public Chunk LoadChunk(int chunkX, int chunkZ)
        {
            int width = 32;
            int depth = 32;

            using (FileStream regionFile = File.OpenRead(this.FilePath))
            {
                byte[] buffer = new byte[8192];

                regionFile.Read(buffer, 0, 8192);

                int xi = (chunkX % width);
                if (xi < 0) xi += 32;
                int zi = (chunkZ % depth);
                if (zi < 0) zi += 32;
                int tableOffset = (xi + zi * width) * 4;

                regionFile.Seek(tableOffset, SeekOrigin.Begin);

                byte[] offsetBuffer = new byte[4];
                regionFile.Read(offsetBuffer, 0, 3);
                Array.Reverse(offsetBuffer);
                int offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;

                byte[] bytes = BitConverter.GetBytes(offset >> 4);
                Array.Reverse(bytes);
                if (offset != 0 && offsetBuffer[0] != bytes[0] && offsetBuffer[1] != bytes[1] && offsetBuffer[2] != bytes[2])
                {
                    throw new FormatException();
                }

                int length = regionFile.ReadByte();

                if (offset == 0 || length == 0)
                {
                    return null;
                }

                regionFile.Seek(offset, SeekOrigin.Begin);
                byte[] waste = new byte[4];
                regionFile.Read(waste, 0, 4);
                int compressionMode = regionFile.ReadByte();

                if (compressionMode != 0x02)
                {
                    throw new FormatException();
                }

                CompoundTag tag = NBTIO.ReadZLIBFile(new BinaryReader(regionFile).ReadBytes((int)(regionFile.Length - regionFile.Position)), NBTEndian.BIG_ENDIAN);
                return this.Provider.DeserializeChunk(tag);
            }
        }

        public void Save(Chunk chunk)
        {
            string filePath = this.FilePath;

            int width = 32;
            int depth = 32;
            
            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);

                using (FileStream regionFile = File.Open(filePath, FileMode.CreateNew))
                {
                    byte[] buffer = new byte[8192];
                    regionFile.Write(buffer, 0, buffer.Length);
                }
            }

            using (FileStream regionFile = File.Open(filePath, FileMode.Open))
            {
                byte[] buffer = new byte[8192];
                regionFile.Read(buffer, 0, buffer.Length);

                int xi = (chunk.X % width);
                if (xi < 0) xi += 32;
                int zi = (chunk.Z % depth);
                if (zi < 0) zi += 32;
                int tableOffset = (xi + zi * width) * 4;

                regionFile.Seek(tableOffset, SeekOrigin.Begin);

                byte[] offsetBuffer = new byte[4];
                regionFile.Read(offsetBuffer, 0, 3);
                Array.Reverse(offsetBuffer);
                int offset = BitConverter.ToInt32(offsetBuffer, 0) << 4;
                byte sectorCount = (byte) regionFile.ReadByte();

                byte[] nbtBuf = NBTIO.WriteZLIBFile(this.Provider.SerializeChunk(chunk), NBTEndian.BIG_ENDIAN);
                int nbtLength = nbtBuf.Length;
                byte nbtSectorCount = (byte) Math.Ceiling(nbtLength / 4096d);

                if (offset == 0 || sectorCount == 0 || nbtSectorCount > sectorCount)
                {
                    regionFile.Seek(0, SeekOrigin.End);
                    offset = (int)((int) regionFile.Position & 0xfffffff0);

                    regionFile.Seek(tableOffset, SeekOrigin.Begin);

                    byte[] bytes = BitConverter.GetBytes(offset >> 4);
                    Array.Reverse(bytes);
                    regionFile.Write(bytes, 0, 3);
                    regionFile.WriteByte(nbtSectorCount);
                }

                byte[] lenghtBytes = BitConverter.GetBytes(nbtLength + 1);
                Array.Reverse(lenghtBytes);

                regionFile.Seek(offset, SeekOrigin.Begin);
                regionFile.Write(lenghtBytes, 0, 4);
                regionFile.WriteByte(0x02);

                regionFile.Write(nbtBuf, 0, nbtBuf.Length);

                Math.DivRem(nbtLength + 4, 4096, out int reminder);

                byte[] padding = new byte[4096 - reminder];
                if (padding.Length > 0) regionFile.Write(padding, 0, padding.Length);
            }
        }
    }
}

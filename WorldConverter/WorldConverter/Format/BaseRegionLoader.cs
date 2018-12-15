using System;
using System.Collections.Generic;
using System.IO;
using MineNET.NBT.Data;
using MineNET.NBT.IO;
using MineNET.NBT.Tags;
using MineNET.Utils;

namespace WorldConverter.Format
{
    public class BaseRegionLoader
    {
        public int X { get; }
        public int Z { get; }

        public ILevelProvider Provider { get; }

        public string FilePath { get; }

        public List<Chunk> Chunks = new List<Chunk>();

        public BaseRegionLoader(ILevelProvider provider, string path)
        {
            this.Provider = provider;

            if (!File.Exists(path))
            {
                throw new Exception($"File {path} not found"); 
            }
            this.FilePath = path;

            string fileName = Path.GetFileName(path);
            fileName = fileName.Remove(0, 2);
            fileName = fileName.Remove(fileName.Length - 4, 4);
            string[] xz = fileName.Split('.');
            this.X = int.Parse(xz[0]);
            this.Z = int.Parse(xz[1]);
        }

        public void LoadChunks()
        {
            int chunkX = (this.X - 1) * 32;
            int chunkZ = (this.Z - 1) * 32;
            for (int x = chunkX; x < chunkX + 32; ++x)
            {
                for (int z = chunkZ; z < chunkZ + 32; ++z)
                {
                    Chunk chunk = this.LoadChunk(x, z);
                    if (chunk != null)
                    {
                        this.Chunks.Add(chunk);
                    }
                }
            }
        }

        public Chunk LoadChunk(int chunkX, int chunkZ)
        {
            int width = 32;
            int depth = 32;

            int rx = chunkX >> 5;
            int rz = chunkZ >> 5;
            Tuple<int, int> regionPos = new Tuple<int, int>(rx, rz);
            Tuple<int, int> chunkPos = new Tuple<int, int>(chunkX, chunkZ);

            FileStream regionFile = File.OpenRead(this.FilePath);
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
            return this.NBTDeserialize(tag);
        }

        public Chunk NBTDeserialize(CompoundTag tag)
        {
            CompoundTag level = tag.GetCompound("").GetCompound("Level");

            int x = level.GetInt("xPos");
            int z = level.GetInt("zPos");

            byte[] biomes = level.GetByteArray("Biomes");
            short[] cast = new short[256];
            int[] heightMap = level.GetIntArray("HeightMap");
            for (int i = 0; i < 256; ++i)
            {
                cast[i] = (short) heightMap[i];
            }
            
            ListTag sections = level.GetList("Sections");
            SubChunk[] subChunks = new SubChunk[sections.Count];
            for (int i = 0; i < sections.Count; ++i)
            {
                CompoundTag section = (CompoundTag) sections[i];
                SubChunk subChunk = new SubChunk();
                byte y = section.GetByte("Y");
                byte[] bytes = section.GetByteArray("Blocks");
                int[] blocks = new int[4096];
                for (int j = 0; j < 4096; ++j)
                {
                    blocks[j] = bytes[j];
                }
                subChunk.BlockDatas = blocks;
                subChunk.MetaDatas = new NibbleArray(section.GetByteArray("Data"));
                subChunk.BlockLight = section.GetByteArray("BlockLight");
                subChunk.SkyLight = section.GetByteArray("SkyLight");
                subChunks[y] = subChunk;
            }

            Chunk chunk = new Chunk(x, z, subChunks, level.GetList("Entities"), level.GetList("TileEntities"))
            {
                LastUpdate = level.GetLong("LastUpdate"),
                LightPopulated = level.GetByte("LightPopulated"),
                TerrainPopulated = level.GetByte("TerrainPopulated"),
                V = level.GetByte("V"),
                InhabitedTime = level.GetLong("InhabitedTime"),
                Biomes = biomes,
                HeightMap = cast
            };
            return chunk;
        }

        public void Convert()
        {
            for (int i = 0; i < this.Chunks.Count; ++i)
            {
                this.Chunks[i].Convert();
            }
        }

        public void Save(string path)
        {
            string filePath = $"{path}\\r.{this.X}.{this.Z}.mca";

            int width = 32;
            int depth = 32;

            if (this.Chunks.Count == 0)
            {
                return;
            }

            Chunk chunk = this.Chunks[0];
            int rx = chunk.X >> 5;
            int rz = chunk.Z >> 5;

            if (!File.Exists(filePath))
            {
                Directory.CreateDirectory(path);

                using (var regionFile = File.Open(filePath, FileMode.CreateNew))
                {
                    byte[] buffer = new byte[8192];
                    regionFile.Write(buffer, 0, buffer.Length);
                }
            }

            using (var regionFile = File.Open(filePath, FileMode.Open))
            {
                byte[] buffer = new byte[8192];
                regionFile.Read(buffer, 0, buffer.Length);

                for (int i = 0; i < this.Chunks.Count; i++)
                {
                    chunk = this.Chunks[i];
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

                    byte[] nbtBuf = NBTIO.WriteZLIBFile(this.NBTSerialize(chunk), NBTEndian.BIG_ENDIAN);
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

                    int reminder;
                    Math.DivRem(nbtLength + 4, 4096, out reminder);

                    byte[] padding = new byte[4096 - reminder];
                    if (padding.Length > 0) regionFile.Write(padding, 0, padding.Length);
                }
            }
        }

        public CompoundTag NBTSerialize(Chunk chunk)
        {
            CompoundTag tag = new CompoundTag();
            tag.PutInt("xPos", chunk.X);
            tag.PutInt("zPos", chunk.Z);

            tag.PutLong("LastUpdate", chunk.LastUpdate);
            tag.PutByte("LightPopulated", chunk.LightPopulated);
            tag.PutByte("TerrainPopulated", chunk.TerrainPopulated);
            tag.PutByte("V", chunk.V);
            tag.PutLong("InhabitedTime", chunk.InhabitedTime);

            tag.PutByteArray("Biomes", chunk.Biomes);
            int[] cast = new int[256];
            chunk.HeightMap.CopyTo(cast, 0);
            tag.PutIntArray("HeightMap", cast);

            ListTag sections = new ListTag("Sections", NBTTagType.COMPOUND);
            SubChunk[] subChunks = chunk.SubChunks;
            for (int i = 0; i < subChunks.Length; ++i)
            {
                CompoundTag data = new CompoundTag();
                data.PutByte("Y", (byte) i);
                byte[] blocks = new byte[subChunks[i].BlockDatas.Length];
                for (int j = 0; j < subChunks[i].BlockDatas.Length; ++j)
                {
                    blocks[j] = (byte) subChunks[i].BlockDatas[j];
                }
                data.PutByteArray("Blocks", blocks);
                data.PutByteArray("Data", subChunks[i].MetaDatas.ArrayData);
                data.PutByteArray("BlockLight", subChunks[i].BlockLight);
                data.PutByteArray("SkyLight", subChunks[i].SkyLight);
                sections.Add(data);
            }
            tag.PutList(sections);

            ListTag entitiesTag = new ListTag("Entities", NBTTagType.COMPOUND);
            CompoundTag[] entities = chunk.EntitiesTag;
            for (int i = 0; i < entities.Length; ++i)
            {
                entitiesTag.Add(entities[i]);
            }
            tag.PutList(entitiesTag);

            ListTag blockEntitiesTag = new ListTag("TileEntities", NBTTagType.COMPOUND);
            CompoundTag[] blockEntities = chunk.BlockEntitiesTag;
            for (int i = 0; i < blockEntities.Length; ++i)
            {
                blockEntitiesTag.Add(blockEntities[i]);
            }
            tag.PutList(blockEntitiesTag);
            
            return new CompoundTag().PutCompound("", new CompoundTag().PutCompound("Level", tag));
        }
    }
}

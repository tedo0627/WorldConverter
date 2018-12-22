using MineNET.NBT.Data;
using MineNET.NBT.Tags;
using MineNET.Utils;

namespace WorldConverter.Format.Region
{
    public class Anvil : RegionLevelProvider
    {
        public override string FileExtension { get; } = "mca";

        public override Chunk DeserializeChunk(CompoundTag tag)
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
            SubChunk[] subChunks = new SubChunk[16];
            for (int i = 0; i < sections.Count; ++i)
            {
                CompoundTag section = (CompoundTag)sections[i];
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

        public override CompoundTag SerializeChunk(Chunk chunk)
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
                if (subChunks[i] == null)
                {
                    continue;
                }
                CompoundTag data = new CompoundTag();
                data.PutByte("Y", (byte)i);
                byte[] blocks = new byte[subChunks[i].BlockDatas.Length];
                for (int j = 0; j < subChunks[i].BlockDatas.Length; ++j)
                {
                    blocks[j] = (byte)subChunks[i].BlockDatas[j];
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

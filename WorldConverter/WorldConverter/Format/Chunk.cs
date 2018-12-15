using MineNET.NBT.Tags;

namespace WorldConverter.Format
{
    public class Chunk
    {
        public int X { get; }
        public int Z { get; }

        public long LastUpdate { get; set; }
        public byte LightPopulated { get; set; }
        public byte TerrainPopulated { get; set; }
        public byte V { get; set; }
        public long InhabitedTime { get; set; }

        public byte[] Biomes { get; set; }
        public short[] HeightMap { get; set; }

        public SubChunk[] SubChunks { get; }

        public CompoundTag[] EntitiesTag { get; }
        public CompoundTag[] BlockEntitiesTag { get; }

        public Chunk(int x, int z, SubChunk[] subChunks, ListTag entitiesTag, ListTag blockEntitiesTag)
        {
            this.X = x;
            this.Z = z;

            this.SubChunks = subChunks;

            this.EntitiesTag = new CompoundTag[entitiesTag.Count];
            for (int i = 0; i < entitiesTag.Count; ++i)
            {
                this.EntitiesTag[i] = (CompoundTag) entitiesTag[i];
            }

            this.BlockEntitiesTag = new CompoundTag[blockEntitiesTag.Count];
            for (int i = 0; i < blockEntitiesTag.Count; ++i)
            {
                this.BlockEntitiesTag[i] = (CompoundTag) blockEntitiesTag[i];
            }
        }

        public void Convert()
        {
            for (int i = 0; i < this.SubChunks.Length; ++i)
            {
                this.SubChunks[i].Convert();
            }
        }
    }
}

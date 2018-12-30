using MineNET.NBT.Tags;
using WorldConverter.Utils;

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
            for (int i = 0; i < this.BlockEntitiesTag.Length; ++i)
            {
                CompoundTag tag = this.BlockEntitiesTag[i];
                switch (tag.GetString("id"))
                {
                    case "minecraft:flower_pot":
                        tag.PutShort("item", (short) Util.GetItemIdFromString(tag.GetString("Item")).Item1);
                        tag.PutInt("mData", tag.GetInt("Data"));

                        tag.Remove("Item");
                        tag.Remove("Data");
                        break;

                    case "minecraft:sign":
                        string text1 = tag.GetString("Text1").Remove(0, 9);
                        text1 = text1.Remove(text1.Length - 2, 2);
                        string text2 = tag.GetString("Text2").Remove(0, 9);
                        text2 = text2.Remove(text2.Length - 2, 2);
                        string text3 = tag.GetString("Text3").Remove(0, 9);
                        text3 = text3.Remove(text3.Length - 2, 2);
                        string text4 = tag.GetString("Text4").Remove(0, 9);
                        text4 = text4.Remove(text4.Length - 2, 2);
                        string text = $"{text1}\n{text2}\n{text3}\n{text4}";
                        tag.PutString("Text", text);
                        break;
                }
            }

            for (int i = 0; i < this.SubChunks.Length; ++i)
            {
                if (this.SubChunks[i] == null)
                {
                    continue;
                }
                this.SubChunks[i].Convert();
            }
        }
    }
}

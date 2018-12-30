using System;
using System.Reflection;
using MineNET.Blocks;
using MineNET.Items;

namespace WorldConverter.Utils
{
    public class Util
    {
        public static Tuple<int, int> GetItemIdFromString(string name)
        {
            string[] data = name.Replace("minecraft:", "").Replace(" ", "_").ToUpper().Split(':');
            int id = 0;
            int meta = 0;

            if (data.Length == 1)
            {
                int.TryParse(data[0], out id);
            }

            if (data.Length == 2)
            {
                int.TryParse(data[0], out id);
                int.TryParse(data[1], out meta);
            }

            ItemIDs ids = new ItemIDs();
            FieldInfo info = ids.GetType().GetField(data[0]);
            if (info != null)
            {
                id = (int) info.GetValue(ids);
            }
            else
            {
                BlockIDs ids2 = new BlockIDs();
                FieldInfo info2 = ids2.GetType().GetField(data[0]);
                if (info2 != null)
                {
                    id = (int) info2.GetValue(ids2);
                    if (id > 255)
                    {
                        id = -id + 255;
                    }
                }
            }
            return new Tuple<int, int>(id, meta);
        }
    }
}

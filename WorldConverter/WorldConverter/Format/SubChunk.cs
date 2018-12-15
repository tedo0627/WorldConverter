using System;
using MineNET.Utils;

namespace WorldConverter.Format
{
    public class SubChunk
    {
        public int[] BlockDatas { get; set; }
        public NibbleArray MetaDatas { get; set; }

        public byte[] BlockLight { get; set; }
        public byte[] SkyLight { get; set; }

        public void Convert()
        {
            for (int x = 0; x < 16; ++x)
            {
                for (int y = 0; y < 16; ++y)
                {
                    for (int z = 0; z < 16; ++z)
                    {
                        int id = this.GetBlockId(x, y, z);
                        int damage = this.GetMetaData(x, y, z);
                        Tuple<int, int> t = this.ChangeIdAndDamage(id, damage);
                        this.SetBlockId(x, y, z, t.Item1);
                        this.SetMetaData(x, y, z, (byte) t.Item2);
                    }
                }
            }
        }

        public int GetArrayIndex(int x, int y, int z)
        {
            return (x * 256) + (z * 16) + y;
        }

        public int GetBlockId(int x, int y, int z)
        {
            return this.BlockDatas[this.GetArrayIndex(x, y, z)];
        }

        public void SetBlockId(int x, int y, int z, int id)
        {
            this.BlockDatas[this.GetArrayIndex(x, y, z)] = id;
        }

        public byte GetMetaData(int x, int y, int z)
        {
            return this.MetaDatas[this.GetArrayIndex(x, y, z)];
        }

        public void SetMetaData(int x, int y, int z, byte meta)
        {
            this.MetaDatas[this.GetArrayIndex(x, y, z)] = meta;
        }

        private Tuple<int, int> ChangeIdAndDamage(int id, int damage)
        {
            switch (id)
            {
                //ポドゾル
                case 3:
                    if (damage == 2)
                    {
                        id = 243;
                        damage = 0;
                    }
                    break;

                //ハーフ
                case 43:
                case 44:
                    if (damage == 6)
                    {
                        damage = 7;
                    }
                    else if (damage == 7)
                    {
                        damage = 6;
                    }
                    else if (damage == 14)
                    {
                        damage = 15;
                    }
                    else if (damage == 15)
                    {
                        damage = 14;
                    }
                    break;

                //ボタン
                case 77:
                case 143:
                    damage = 6 - damage;
                    break;

                //色付きガラス
                case 95:
                    id = 241;
                    break;

                //トラップドア
                case 96:
                case 167:
                    if (damage < 4)
                    {
                        damage = 3 - damage;
                    }
                    else if (damage < 8)
                    {
                        damage = 7 - damage + 8;
                    }
                    else if (damage < 12)
                    {
                        damage = 11 - damage + 4;
                    }
                    else
                    {
                        damage = 15 - damage + 12;
                    }
                    break;

                //ハーフ
                case 125:
                    id = 157;
                    break;
                case 126:
                    id = 158;
                    break;
                
                //レール
                case 157:
                    id = 126;
                    break;

                //ドロッパー
                case 158:
                    id = 125;
                    break;

                //バリアブロック
                case 166:
                    id = 95;
                    break;

                //フェンス
                case 188:
                case 189:
                case 190:
                    damage = id - 187;
                    id = 85;
                    break;
                case 191:
                    id = 85;
                    damage = 5;
                    break;
                case 192:
                    id = 85;
                    damage = 4;
                    break;

                //エンド系
                case 198:
                    id = 208;
                    if (damage == 4)
                    {
                        damage = 5;
                    }
                    else if (damage == 5)
                    {
                        damage = 4;
                    }
                    break;
                case 199:
                    id = 240;
                    break;
                case 204:
                case 205:
                    id = id - 23;
                    damage = 1;
                    break;

                //ビートルート
                case 207:
                    id = 244;
                    break;

                //道ブロック
                case 208:
                    id = 198;
                    break;

                //コマンドブロック
                case 210:
                case 211:
                    id = id - 22;
                    break;

                //氷塊
                case 212:
                    id = 207;
                    break;

                //オブザーバー
                case 218:
                    id = 151;
                    break;

                //シュルカーボックス
                case 219:
                case 220:
                case 221:
                case 222:
                case 223:
                case 224:
                case 225:
                case 226:
                case 227:
                case 228:
                case 230:
                case 231:
                case 232:
                case 233:
                case 234:
                    damage = id - 219;
                    id = 218;
                    break;

                case 229:
                    id = 205;
                    break;

                //テラコッタ
                case 235:
                case 236:
                case 237:
                case 238:
                case 239:
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                case 250:
                    id = id - 15;
                    break;

                //コンクリート
                case 251:
                    id = 236;
                    break;

                //コンクリートパウダー
                case 252:
                    id = 237;
                    break;

                //ストラクチャーブロック
                case 255:
                    id = 252;
                    break;
            }
            return new Tuple<int, int>(id, damage);
        }
    }
}

﻿using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class GlobalValues : Chunk
    {
        public int[] Values = new int[0];

        public GlobalValues()
        {
            ChunkName = "GlobalValues";
            ChunkID = 0x2232;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Values = new int[reader.ReadShort()];
            for (int i = 0; i < Values.Length; i++)
                Values[i] = reader.ReadInt();

            NebulaCore.PackageData.GlobalValues = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Values = new int[reader.ReadInt()];
            ((GlobalValueNames)extraInfo[0]).Names = new string[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                ((GlobalValueNames)extraInfo[0]).Names[i] = reader.ReadAutoYuniversal();
                reader.Skip(4); // Type (Always int)
                Values[i] = reader.ReadInt();
            }
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Values.Length);
            for (int i = 0; i < Values.Length; i++)
            {
                writer.WriteAutoYunicode(((GlobalValueNames)extraInfo[0]).Names.Length > i ? ((GlobalValueNames)extraInfo[0]).Names[i] : "");
                writer.WriteInt(0);
                writer.WriteInt(Values[i]);
            }
        }
    }
}

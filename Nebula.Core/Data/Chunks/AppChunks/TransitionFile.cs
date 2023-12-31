﻿using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.AppChunks
{
    public class TransitionFile : Chunk
    {
        public short Handle;
        public string Name = string.Empty;

        public TransitionFile()
        {
            ChunkName = "TransitionFile";
            ChunkID = 0x2231;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadShort();
            Name = reader.ReadYuniversal();

            NebulaCore.PackageData.TransitionFile = this;
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {

        }
    }
}

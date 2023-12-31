﻿using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks
{
    public class FrameHandles : Chunk
    {
        public short[] FrameHandleIndex = new short[0];

        public FrameHandles()
        {
            ChunkName = "FrameHandles";
            ChunkID = 0x222B;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            var count = reader.Size() / 2;
            FrameHandleIndex = new short[count];

            for (int i = 0; i < count; i++)
                FrameHandleIndex[i] = reader.ReadShort();

            NebulaCore.PackageData.FrameHandles = FrameHandleIndex.ToList();
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

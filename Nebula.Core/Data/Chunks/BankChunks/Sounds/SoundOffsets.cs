﻿using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.BankChunks.Sounds
{
    public class SoundOffsets : Chunk
    {
        public SoundOffsets()
        {
            ChunkName = "SoundOffsets";
            ChunkID = 0x5557;
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            NebulaCore.PackageData.SoundOffsets = this;
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

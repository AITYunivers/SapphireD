﻿using Nebula.Core.Memory;

namespace Nebula.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ExpressionInt : ExpressionChunk
    {
        public int Value;

        public ExpressionInt()
        {
            ChunkName = "ExpressionInt";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            Value = reader.ReadInt();
        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteInt(Value);
        }
    }
}

﻿using SapphireD.Core.Memory;

namespace SapphireD.Core.Data.Chunks.FrameChunks.Events.Parameters
{
    public class ParameterObject : ParameterChunk
    {
        public short ObjectInfoList;
        public short ObjectInfo;
        public short ObjectType;

        public ParameterObject()
        {
            ChunkName = "ParameterObject";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            ObjectInfoList = reader.ReadShort();
            ObjectInfo = reader.ReadShort();
            ObjectType = reader.ReadShort();
        }
    }
}
﻿using SapphireD.Core.Memory;
using System.Diagnostics;

namespace SapphireD.Core.Data.Chunks.BankChunks.Sounds
{
    public class Sound : Chunk
    {
        public uint Handle;
        public int Checksum;
        public uint References;
        public BitDict Flags = new BitDict( // Flags
            "", "", "", "", "", "Decompressed", // Decompressed
            "", "", "HasName",                  // Has Name (Android Only?)
            "", "", "", "", "", "NameCrop"      // Name Crop
        );
        public int Frequency;
        public string Name = string.Empty;
        public byte[] Data = new byte[0];
        public bool Compressed = true;

        public Sound()
        {
            ChunkName = "Sound";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {
            int decompressedSize;
            int nameLength;
            ByteReader soundData;
            if (SapDCore.Android)
            {
                Handle = reader.ReadUShort();
                Flags.Value = reader.ReadUShort();
                reader.Skip(4);
                Frequency = reader.ReadInt();
                if (Flags["HasName"])
                    Name = reader.ReadYuniversal(reader.ReadShort());
                return;
            }
            else if (SapDCore.iOS)
            {
                Handle = reader.ReadUShort();
                Name = reader.ReadYuniversal(reader.ReadShort());
                reader.Skip(4);
                Data = reader.ReadBytes(reader.ReadInt());
                return;
            }
            else if (SapDCore.Flash)
            {
                Handle = reader.ReadUShort();
                Name = reader.ReadYuniversal(reader.ReadShort());
                return;
            }
            else if (SapDCore.HTML)
            {
                Handle = reader.ReadUShort();
                reader.Skip(1);
                Frequency = reader.ReadInt();
                Name = reader.ReadYuniversal(reader.ReadShort());
                return;
            }

            Handle = reader.ReadUInt();
            if (SapDCore.Fusion >= 2.5f)
                Handle--;
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            decompressedSize = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Frequency = reader.ReadInt();
            nameLength = reader.ReadInt();
            if (Compressed && !Flags["Decompressed"])
            {
                int size = reader.ReadInt();
                soundData = new ByteReader(Decompressor.DecompressBlock(reader, size));
            }
            else
                soundData = new ByteReader(reader.ReadBytes(decompressedSize));
            Name = soundData.ReadYuniversalStop(nameLength);
            if (Flags["Decompressed"]) soundData.Seek(0);
            Data = soundData.ReadBytes();
        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            Handle = reader.ReadUInt() - 1;
            Checksum = reader.ReadInt();
            References = reader.ReadUInt();
            int decompressedSize = reader.ReadInt();
            Flags.Value = reader.ReadUInt();
            Frequency = reader.ReadInt();
            int nameLength = reader.ReadInt();
            ByteReader soundData;
            soundData = new ByteReader(reader.ReadBytes(decompressedSize));
            Name = soundData.ReadWideString(nameLength);
            if (Flags["Decompressed"]) soundData.Seek(0);
            Data = soundData.ReadBytes();
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            Flags["NameCrop"] = false;
            writer.WriteUInt(Handle + 1);
            writer.WriteInt(Checksum);
            writer.WriteUInt(References);
            writer.WriteInt(Data.Length + (Name.Length * 2));
            writer.WriteUInt(Flags.Value);
            writer.WriteInt(Frequency);
            writer.WriteInt(Name.Length);

            if (Flags["Decompressed"])
                writer.WriteBytes(Data);
            else
            {
                writer.WriteUnicode(Name);
                writer.WriteBytes(Data);
            }
        }
    }
}

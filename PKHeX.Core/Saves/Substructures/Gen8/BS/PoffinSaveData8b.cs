﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Details about the Poketch corner app.
    /// </summary>
    /// <remarks>size: 0x644</remarks>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class PoffinSaveData8b : SaveBlock
    {
        public PoffinSaveData8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        // structure:
        // PoffinData[] Poffins;
        // int CookingCount;

        // 0x640 bytes of data is for poffins
        public const int COUNT_POFFIN = 100;

        private int GetPoffinOffset(int index)
        {
            if ((uint)index >= COUNT_POFFIN)
                throw new ArgumentOutOfRangeException(nameof(index));
            return Offset + (index * Poffin8b.SIZE);
        }

        public Poffin8b GetPoffin(int index) => new(Data, GetPoffinOffset(index));
        public void SetPoffin(int index, Poffin8b poffin) => poffin.CopyTo(Data, GetPoffinOffset(index));
        public int CookingCount { get => BitConverter.ToInt32(Data, Offset + 0x640); set => BitConverter.GetBytes(value).CopyTo(Data, Offset + 0x640); }

        public Poffin8b[] GetPoffins()
        {
            var result = new Poffin8b[COUNT_POFFIN];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetPoffin(i);
            return result;
        }

        public void SetPoffins(IReadOnlyCollection<Poffin8b> value)
        {
            if (value.Count != COUNT_POFFIN)
                throw new ArgumentOutOfRangeException(nameof(value.Count));
            var ordered = value.OrderBy(z => z.IsNull).ThenBy(z => z.IsNew);
            int ctr = 0;
            foreach (var p in ordered)
                SetPoffin(ctr++, p);
        }

        public int CountPoffins() => GetPoffins().Count(z => z.IsNull);
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class Poffin8b
    {
        public const int SIZE = 0x10;

        private readonly byte[] Data = new byte[SIZE];

        public Poffin8b(byte[] data, int offset)
        {
            data.AsSpan(offset, SIZE).CopyTo(Data);
        }

        public void Clear() => Data.AsSpan().Clear();
        public void CopyTo(byte[] data, int offset) => Data.CopyTo(data, offset);
        public void ToNull() => MstID = 0xFF;
        public bool IsNull => MstID == 0xFF;

        public byte MstID { get => Data[0x00]; set => Data[0x00] = value; }
        public byte Level { get => Data[0x01]; set => Data[0x01] = value; }
        public byte Taste { get => Data[0x02]; set => Data[0x02] = value; } // Smoothness/feel of the Poffin => +sheen

        public bool IsNew { get => BitConverter.ToUInt32(Data, 0x04) == 1; set => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, 0x04); }

        public byte FlavorSpicy  { get => Data[0x08]; set => Data[0x08] = value; }
        public byte FlavorDry    { get => Data[0x09]; set => Data[0x09] = value; }
        public byte FlavorSweet  { get => Data[0x0A]; set => Data[0x0A] = value; }
        public byte FlavorBitter { get => Data[0x0B]; set => Data[0x0B] = value; }
        public byte FlavorSour   { get => Data[0x0C]; set => Data[0x0C] = value; }
    }
}

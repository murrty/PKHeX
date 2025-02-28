namespace PKHeX.Core
{
    /// <summary>
    /// Locks associated to a given NPC PKM that appears before a <see cref="EncounterStaticShadow"/>.
    /// </summary>
    public readonly struct NPCLock
    {
        private readonly int Species;
        private readonly byte Nature;
        private readonly byte Gender;
        private readonly byte Ratio;
        private readonly byte State;

        public int FramesConsumed => Seen ? 5 : 7;
        public bool Seen => State > 1;
        public bool Shadow => State != 0;

        // Not-Shadow
        public NPCLock(short s, byte n, byte g, byte r)
        {
            Species = s;
            Nature = n;
            Gender = g;
            Ratio = r;
            State = 0;
        }

        // Shadow
        public NPCLock(short s, bool seen = false)
        {
            Species = s;
            Nature = 0;
            Gender = 0;
            Ratio = 0;
            State = seen ? (byte)2 : (byte)1;
        }

        public bool MatchesLock(uint PID)
        {
            if (Shadow && Nature == 0) // Non-locked shadow
                return true;
            if (Gender != 2 && Gender != ((PID & 0xFF) < Ratio ? 1 : 0))
                return false;
            if (Nature != PID % 25)
                return false;
            return true;
        }

        public override bool Equals(object obj) => false;
        public override int GetHashCode() => 0;
        public static bool operator ==(NPCLock left, NPCLock right) => left.Equals(right);
        public static bool operator !=(NPCLock left, NPCLock right) => !(left == right);

#if DEBUG
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder(64);
            sb.Append((Species)Species);
            if (State != 0)
                sb.Append(" (Shadow)");
            if (Seen)
                sb.Append(" [Seen]");
            sb.Append(" - ");
            sb.Append("Nature: ").Append((Nature)Nature);
            if (Gender != 2)
                sb.Append(", ").Append("Gender: ").Append(Gender);
            return sb.ToString();
        }
#endif
    }
}

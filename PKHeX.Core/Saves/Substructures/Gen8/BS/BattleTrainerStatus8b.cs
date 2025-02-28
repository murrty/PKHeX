﻿using System;

namespace PKHeX.Core
{
    /// <summary>
    /// Defeated Status for all trainers (Dpr.Trainer.TrainerID)
    /// </summary>
    /// <remarks>size: 0x1618</remarks>
    public sealed class BattleTrainerStatus8b : SaveBlock
    {
        public BattleTrainerStatus8b(SAV8BS sav, int offset) : base(sav) => Offset = offset;

        // Structure:
        // (bool IsWin, bool IsBattleSearcher)[707];
        private const int COUNT_TRAINER = 707;
        private const int SIZE_TRAINER = 8; // bool,bool

        /// <summary>
        /// Don't use this unless you've finished the post-game.
        /// </summary>
        public void DefeatAll()
        {
            for (int i = 0; i < COUNT_TRAINER; i++)
            {
                SetIsWin(i, true);
                SetIsBattleSearcher(i, false);
            }
        }

        /// <summary>
        /// Don't use this unless you've finished the post-game.
        /// </summary>
        public void RebattleAll()
        {
            for (int i = 0; i < COUNT_TRAINER; i++)
            {
                SetIsWin(i, true);
                SetIsBattleSearcher(i, true);
            }
        }

        private int GetTrainerOffset(int trainer)
        {
            if ((uint)trainer >= COUNT_TRAINER)
                throw new ArgumentOutOfRangeException(nameof(trainer));
            return Offset + (trainer * SIZE_TRAINER);
        }

        public bool GetIsWin(int trainer) => BitConverter.ToUInt32(Data, GetTrainerOffset(trainer)) == 1;
        public bool GetIsBattleSearcher(int trainer) => BitConverter.ToUInt32(Data, GetTrainerOffset(trainer) + 4) == 1;
        public void SetIsWin(int trainer, bool value) => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, GetTrainerOffset(trainer));
        public void SetIsBattleSearcher(int trainer, bool value) => BitConverter.GetBytes(value ? 1u : 0u).CopyTo(Data, GetTrainerOffset(trainer) + 4);
    }
}

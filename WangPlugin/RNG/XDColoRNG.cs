using PKHeX.Core;
using System;

namespace WangPlugin
{
    internal static class XDColoRNG
    {
        private const int shift = 16;
        private const int shift8 = 8;

        public static uint Next(uint seed) => PKHeX.Core.RNG.XDRNG.Next(seed);
        public static bool GenPkm(ref PKM pk, uint seed,bool[] shiny, bool[] IV, uint Xor = 0)
        {
            int FlawlessIVs = 0;
            uint MAX = 31;
            var rng = RNG.XDRNG;
            var la = new LegalityAnalysis(pk);
            switch (pk.Species)
                {
                    case (int)Species.Umbreon or (int)Species.Eevee: // Colo Umbreon, XD Eevee
                        pk.TID = (int)((seed = rng.Next(seed)) >> 16);
                        pk.SID = (int)((seed = rng.Next(seed)) >> 16);
                        seed = rng.Advance(seed, 2); // PID calls consumed
                        break;
                    case (int)Species.Espeon: // Colo Espeon
                        pk.TID = (int)((seed = rng.Next(seed)) >> 16);
                        pk.SID = (int)((seed = rng.Next(seed)) >> 16);
                        seed = rng.Advance(seed, 9); // PID calls consumed, skip over Umbreon
                        break;
                }
                var A = rng.Next(seed); // IV1
                var B = rng.Next(A); // IV2
                var C = rng.Next(B); // Ability?
                var D = rng.Next(C); // PID
                var E = rng.Next(D); // PID
                pk.PID = (D & 0xFFFF0000) | E >> 16;
                if(!CheckShiny(pk.PID,pk.TID,pk.SID,shiny, Xor))
                {
                    return false;
                }
                Span<int> IVs = stackalloc int[6];
                GetIVsInt32(IVs, A >> 16, B >> 16);
            if (IV[0] && IVs[1] != 0)
            {
                return false;
            }
            if (IV[1] && IVs[3] != 0)
            {
                return false;
            }
            if (IV[2] == true)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (IVs[i] == MAX)
                        FlawlessIVs++;
                }
                if (FlawlessIVs is 3 or > 3)
                {
                    pk.SetIVs(IVs);
                    pk.Nature = (int)(pk.PID % 100 % 25);
                    pk.RefreshAbility((int)(pk.PID & 1));
                     la = new LegalityAnalysis(pk);
                    if (!la.Info.PIDIVMatches)
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            pk.SetIVs(IVs);

                pk.Nature = (int)(pk.PID % 100 % 25);
                pk.RefreshAbility((int)(pk.PID & 1));
                la = new LegalityAnalysis(pk);
                if (!la.Info.PIDIVMatches)
                {
                    return false;
                }
            return true;
        }
        private static void GetIVsInt32(Span<int> result, uint r1, uint r2)
        {
            result[5] = (((int)r2 >> 10) & 0x1F);
            result[4] = (((int)r2 >> 5) & 0x1F);
            result[3] = (int)(r2 & 0x1F);
            result[2] = (((int)r1 >> 10) & 0x1F);
            result[1] = (((int)r1 >> 5) & 0x1F);
            result[0] = (int)(r1 & 0x1F);
        }
        private static bool CheckShiny(uint pid, int TID, int SID, bool[] shiny, uint Xor = 0)
        {
            var s = (uint)(TID ^ SID) ^ ((pid >> 16) ^ (pid & 0xFFFF));
            if (shiny[0])
                return true;
            else if (shiny[1] && s < 16)
                return true;
            else if (shiny[2] && s < 16 && s != 0)
                return true;
            else if (shiny[3] && s == 0)
                return true;
            else if (shiny[4] && s == 1)
                return true;
            else if (shiny[5] && s == Xor)
                return true;
            else
                return false;
        }

    }
}
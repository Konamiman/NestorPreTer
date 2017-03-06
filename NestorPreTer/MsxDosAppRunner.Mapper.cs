using System;
using System.Collections.Generic;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorPreTer
{
    public partial class MsxDosAppRunner
    {
        private Dictionary<int, Action> MapperCalls;

        void InitializeMapperCalls()
        {
            MapperCalls = new Dictionary<int, Action>
            {
                {0xFFCA, EXTBIO},
                {0xF200, ALL_SEG},
                {0xF212, PUT_PH},
                {0xF233, GET_P1},
                {0xF239, GET_P2}
            };

            foreach (var address in MapperCalls.Keys)
            {
                mem[address] = 0xC3;
                mem[address + 1] = (byte)(address & 0xFF);
                mem[address + 2] = (byte)((address>>8) & 0xFF);
            }

            ResetMapper();
        }

        private void ExecuteMapperCall()
        {
            if (MapperCalls.ContainsKey(r.PC))
                MapperCalls[r.PC]();
        }

        private void EXTBIO()
        {
            if (r.DE == 0x0402)
                r.HL = 0xF200.ToShort();
        }

        private byte NextSegmentToAllocate;

        private void ResetMapper()
        {
            NextSegmentToAllocate = 4;
        }

        private void ALL_SEG()
        {
            r.CF = 0;
            r.B = NextSegmentToAllocate;
            NextSegmentToAllocate++;
        }

        private void GET_P1()
        {
            r.A = (byte)mem.GetBlockInBank(1);
        }

        private void GET_P2()
        {
            r.A = (byte)mem.GetBlockInBank(2);
        }

        private void PUT_PH()
        {
            var page = r.H >> 6;
            mem.SetBankValue(page, r.A);
        }
    }
}

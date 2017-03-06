using System;
using System.Collections.Generic;

namespace Konamiman.NestorPreTer
{
    partial class MsxDosAppRunner
    {
        private Dictionary<byte, Action> DosCalls;

        private void InitializeDosCalls()
        {
            DosCalls = new Dictionary<byte, Action>
            {
                {0x02, CONOUT},
                {0x09, STROUT}
            };
        }

        private void ExecuteDosCall()
        {
            if (DosCalls.ContainsKey(r.C))
                DosCalls[r.C]();
        }

        private void CONOUT()
        {
            Console.Write(Convert.ToChar(r.E));
        }

        private void STROUT()
        {
            const byte dollarSign = 36;

            var pointer = r.DE;
            byte theChar;
            while ((theChar = mem[pointer]) != dollarSign)
            {
                Console.Write(Convert.ToChar(theChar));
                pointer++;
            }
        }
    }
}

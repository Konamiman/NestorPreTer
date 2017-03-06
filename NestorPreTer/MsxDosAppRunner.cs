using System.Text;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorPreTer
{
    public partial class MsxDosAppRunner
    {
        private IZ80Processor z80;
        private IZ80Registers r;
        private IMemory mem;

        public MsxDosAppRunner(byte[] application)
        {
            z80 = new Z80Processor();
            z80.AutoStopOnRetWithStackEmpty = true;
            z80.ClockSynchronizer = null;

            r = z80.Registers;
            mem = z80.Memory;

            z80.Memory.SetContents(0x100, application);

            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;

            InitializeDosCalls();
        }

        private void Z80_BeforeInstructionFetch(object sender, BeforeInstructionFetchEventArgs e)
        {
            if (r.PC == 5)
            {
                if (r.C == 0)
                {
                    e.ExecutionStopper.Stop();
                }
                else
                {
                    ExecuteDosCall();
                    z80.ExecuteRet();
                }
            }
        }

        public int Run(string commandLineArgs)
        {
            mem[0x80] = (byte)commandLineArgs.Length;
            if(commandLineArgs.Length > 0)
                mem.SetContents(0x81, Encoding.ASCII.GetBytes(commandLineArgs));

            z80.Reset();
            r.PC = 0x100;
            z80.Continue();
            return 0;
        }
    }
}

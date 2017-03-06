using System.Linq;
using System.Text;
using Konamiman.NestorMSX.Memories;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorPreTer
{
    public partial class MsxDosAppRunner
    {
        private IZ80Processor z80;
        private IZ80Registers r;
        private MappedRam mem;

        public MsxDosAppRunner(byte[] application)
        {
            z80 = new Z80Processor();
            z80.AutoStopOnRetWithStackEmpty = true;
            z80.ClockSynchronizer = null;

            r = z80.Registers;
            mem = new MappedRam(512 / 16);
            z80.Memory = mem;

            mem.SetBankValue(0, 3);
            mem.SetBankValue(1, 2);
            mem.SetBankValue(2, 1);
            mem.SetBankValue(3, 0);

            for (int i = 0; i < application.Length; i++)
                mem[0x100 + i] = application[i];

            mem[0xF342] = 2; //RAM segment in page 1

            z80.BeforeInstructionFetch += Z80_BeforeInstructionFetch;

            InitializeDosCalls();
            InitializeMapperCalls();
        }

        private int[] AddressesToIgnoreExecution = new[]
        {
            0x0024 //ENASLT
        };

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

            else if (r.PC >= 0xF000)
            {
                ExecuteMapperCall();
                z80.ExecuteRet();
            }

            else if (AddressesToIgnoreExecution.Contains(r.PC))
            {
                z80.ExecuteRet();
            }
        }

        public int Run(string commandLineArgs)
        {
            var commandLineBytes = Encoding.ASCII.GetBytes(commandLineArgs);
            mem[0x80] = (byte)commandLineArgs.Length;
            for (int i = 0; i < commandLineBytes.Length; i++)
                mem[0x81 + i] = commandLineBytes[i];

            z80.Reset();
            r.PC = 0x100;
            z80.Continue();

            ResetMapper();
            return 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Konamiman.Z80dotNet;

namespace Konamiman.NestorPreTer
{
    partial class MsxDosAppRunner
    {
        private Dictionary<byte, Action> DosCalls;
        private FileStream[] OpenFiles = new FileStream[5];

        private void InitializeDosCalls()
        {
            DosCalls = new Dictionary<byte, Action>
            {
                {0x02, CONOUT},
                {0x09, STROUT},
                {0x43, OPEN},
                {0x44, CREATE},
                {0x45, CLOSE},
                {0x48, READ},
                {0x49, WRITE },
                {0x4A, SEEK },
                {0x5B, PARSE},
                {0x6F, GETVER}
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

        private void OPEN()
        {
            var fh = GetFreeFileHandle();
            var fileName = ExtractStringFromMemory(r.DE);
            var file = File.Open(fileName, FileMode.Open);
            OpenFiles[fh] = file;

            r.A = 0;
            r.B = (byte)fh;
        }

        private void CREATE()
        {
            var fh = GetFreeFileHandle();
            var fileName = ExtractStringFromMemory(r.DE);
            var file = File.Create(fileName);
            OpenFiles[fh] = file;

            r.A = 0;
            r.B = (byte)fh;
        }

        private void CLOSE()
        {
            r.A = 0;

            var fh = r.B;
            if (fh >= OpenFiles.Length)
                return;

            var file = OpenFiles[fh];
            if (file == null)
                return;

            file.Close();
            OpenFiles[fh] = null;
        }

        private void READ()
        {
            r.A = 0;
            var length = r.HL;
            if (length == 0)
                return;

            var fh = r.B;
            if (fh >= OpenFiles.Length)
                throw new Exception("File not open");

            var file = OpenFiles[r.B];
            if(file == null)
                throw new Exception("File not open");

            var buffer = new byte[length];
            var actualLength = file.Read(buffer, 0, length);

            var pointer = r.DE;
            for (int i = 0; i < actualLength; i++)
            {
                mem[pointer] = buffer[i];
                pointer++;
            }

            r.HL = actualLength.ToShort();
        }

        private void WRITE()
        {
            r.A = 0;
            var length = r.HL;
            if (length == 0)
            {
                r.HL = 0;
                return;
            }

            var fh = r.B;
            if (fh >= OpenFiles.Length)
                throw new Exception("File not open");

            var file = OpenFiles[r.B];
            if (file == null)
                throw new Exception("File not open");

            var pointer = r.DE;
            var buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = mem[pointer];
                pointer++;
            }

            file.Write(buffer, 0, length);
            
            r.HL = length;
        }

        private void SEEK()
        {
            r.A = 0;

            var fh = r.B;
            if (fh >= OpenFiles.Length)
                throw new Exception("File not open");

            var file = OpenFiles[r.B];
            if (file == null)
                throw new Exception("File not open");

            //Used only to rewind file, so ignore r.A (seek method)

            long offset = r.HL + (r.DE << 16);
            file.Seek(offset, SeekOrigin.Begin);
        }

        private int GetFreeFileHandle()
        {
            for(int i=0; i<OpenFiles.Length; i++)
                if (OpenFiles[i] == null) return i;

            throw new Exception("Too many files open");
        }

        private void PARSE()
        {
            var input = ExtractStringFromMemory(r.DE);
            var fileName = Path.GetFileName(input);
            var notFileName = input.Substring(0, input.Length - fileName?.Length ?? 0);
            var notFileNameLengthInBytes = Encoding.ASCII.GetBytes(notFileName).Length;
            r.HL = (r.DE + notFileNameLengthInBytes).ToShort();
        }


        private void GETVER()
        {
            r.AF = 0;
            r.BC = 0x0220;
        }

        private string ExtractStringFromMemory(int address)
        {
            var bytes = new List<byte>();
            byte theByte;
            while ((theByte = mem[address]) != 0)
            {
                bytes.Add(theByte);
                address++;
            }
            return Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}

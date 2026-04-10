using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Chip_8
{
    internal class Chip8
    {
        Random rng = new Random();
        private byte[] memory = new byte[4096];
        private byte[] register = new byte[16];
        private ushort[] stack = new ushort[16];
        private ushort pc;
        private ushort sp = 0;
        private ushort indexRegister;
        private bool[,] display = new bool[64, 32];
        private bool[] key = new bool[16];
        private byte delayTimer;
        private byte soundTimer;
        private byte[] fontSet =
        {
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80  // F
    };
        public bool GetDisplay(int x, int y)
        {
            return display[x, y];
        }

        public void SetKey(byte i, bool setter)
        {
            key[i] = setter;
        }
        public Chip8()
        {
            pc = 0x200;

            for (int i = 0; i < fontSet.Length; i++)
            {
                memory[i] = fontSet[i];
            }
        }
        public void LoadRom(string filePath)
        {
            byte[] romBytes = File.ReadAllBytes(filePath);

            for (int i = 0; i < romBytes.Length; i++)
            {
                memory[0x200 + i] = romBytes[i];
            }
        }

        public void UpdateTimer()
        {
            if (delayTimer > 0)
            {
                delayTimer--;
            }

            if (soundTimer > 0)
            {
                soundTimer--;
            }
        }


        public void Cycle()
        {
            //Fetch
            ushort highByte = memory[pc];
            ushort lowByte = memory[pc + 1];

            highByte = (ushort)(highByte << 8);

            ushort opcode = (ushort)(highByte | lowByte);
            pc += 2;

            //Decode & Execute
            ushort nibble1 = (ushort)(opcode >> 12);

            switch (nibble1)
            {
                case 0x0:
                    case0x0(opcode);
                    break;
                case 0x1:
                    //Jumps to address in memory
                    pc = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x2:
                    //0x2nnn, stores pc in stack, increments sc, stores nnn in pc
                    case0x2(opcode);
                    break;
                case 0x3:
                    //0x3xkk, increments pc by 2 if Vx == kk
                    case0x3(opcode);
                    break;
                case 0x4:
                    //0x4xkk, increments pc by 2 if Vx != kk
                    case0x4(opcode);
                    break;
                case 0x5:
                    //0x4xy0, increments pc by 2 if Vx == Vy
                    case0x5(opcode);
                    break;
                case 0x6:
                    //0x6xkk, stores kk in x register
                    register[(opcode >> 8) & 0x000F] = (byte)(opcode & 0x00FF);
                    break;
                case 0x7:
                    //0x7xkk, adds kk to Vx
                    case0x7(opcode);
                    break;
                case 0x8:
                    case0x8(opcode);
                    break;
                case 0x9:
                    //0x9xy0, increments pc by 2 if Vx != Vy
                    case0x9(opcode);
                    break;
                case 0xA:
                    //0xAnnn, sets indexRegister to nnn
                    indexRegister = (ushort)(opcode & 0x0FFF);
                    break;
                case 0xB:
                    //0xBnnn, jumps pc to nnn + V0
                    int nnn = opcode & 0x0FFF;
                    pc = (ushort)(nnn + register[0]);
                    break;
                case 0xC:
                    //0xCxkk, Vx = random byte & kk
                    case0xC(opcode);
                    break;
                case 0xD:
                    //0xDxyn, x and y are indexes of registers, n is height of sprite in rows
                    case0xD(opcode);
                    break;
                case 0xE:
                    case0xE(opcode);
                    break;
                case 0xF:
                    case0xF(opcode);
                    break;
            }
        }
        public void case0x0(ushort opcode)
        {
            switch (opcode & 0x00FF)
            {
                case 0x00E0:
                    //Clear the display
                    Array.Clear(display, 0, display.Length);
                    break;
                case 0x00EE:
                    //Return from a subroutine
                    sp--;
                    pc = stack[sp];
                    break;
            }
        }
        public void case0x2(ushort opcode)
        {
            ushort value = (ushort)(opcode & 0x0FFF);
            stack[sp] = pc;
            sp++;
            pc = value;
        }

        public void case0x3(ushort opcode)
        {
            int Vx;
            int kk;
            Vx = register[(opcode >> 8) & 0x000F];
            kk = (opcode & 0x00FF);
            if (Vx == kk)
            {
                pc += 2;
            }
        }
        public void case0x4(ushort opcode)
        {
            int Vx;
            int kk;
            Vx = register[(opcode >> 8) & 0x000F];
            kk = (opcode & 0x00FF);
            if (Vx != kk)
            {
                pc += 2;
            }
        }
        public void case0x5(ushort opcode)
        {
            int Vx;
            int Vy;
            Vx = register[(opcode >> 8) & 0x000F];
            Vy = register[(opcode >> 4) & 0x000F];
            if (Vx == Vy)
            {
                pc += 2;
            }
        }

        public void case0x7(ushort opcode)
        {
            int x;
            int kk;
            x = (opcode >> 8) & 0x000F;
            kk = opcode & 0x00FF;
            register[x] = (byte)(register[x] + kk);
        }
        public void case0x8(ushort opcode)
        {
            int x;
            int y;
            byte flag;
            int subtraction;
            switch (opcode & 0x000F)
            {
                //Math operators
                case 0x0:
                    //0x8xy0, sets Vx = Vy
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    register[x] = register[y];
                    break;
                case 0x1:
                    //0x8xy1, sets Vx = Vx OR Vy
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    int valOr = register[x] | register[y];
                    register[x] = (byte)valOr;
                    break;
                case 0x2:
                    //0x8xy2, sets Vx = Vx and Vy
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    int valAnd = register[x] & register[y];
                    register[x] = (byte)valAnd;
                    break;
                case 0x3:
                    //0x8xy3, sets Vx = Vx ^ Vy
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    int valXor = register[x] ^ register[y];
                    register[x] = (byte)valXor;
                    break;
                case 0x4:
                    //0x8xy4, sets Vx = Vx + Vy, sets VF = 1 if overflow
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    int summation = register[x] + register[y];
                    flag = 0;
                    if (summation > 255)
                    {
                        flag = 1;
                    }
                    register[x] = (byte)summation;
                    register[0xF] = flag;
                    break;
                case 0x5:
                    //0x8xy5, sets Vx = Vx - Vy, sets VF = 1 if Vx > Vy
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    subtraction = register[x] - register[y];
                    flag = 0;
                    if (register[x] >= register[y])
                    {
                        flag = 1;
                    }
                    register[x] = (byte)subtraction;
                    register[0xF] = flag;
                    break;
                case 0x6:
                    //0x8xy6, set Vx = Vx SHR 1
                    x = (opcode >> 8) & 0x000F;
                    flag = 0;
                    if ((register[x] & 1) == 1)
                    {
                        flag = 1;
                    }
                    register[x] /= 2;
                    register[0xF] = flag;
                    break;
                case 0x7:
                    //0x8xy7, Vx = Vy - Vx, VF = 1 if Vy >= Vx
                    x = (opcode >> 8) & 0x000F;
                    y = (opcode >> 4) & 0x000F;
                    subtraction = register[y] - register[x];
                    flag = 0;
                    if (register[y] >= register[x])
                    {
                        flag = 1;
                    }
                    register[x] = (byte)subtraction;
                    register[0xF] = flag;
                    break;
                case 0xE:
                    //0x8xyE, Vx *= 2, VF = 1 if most significant digit of Vx = 1
                    x = (opcode >> 8) & 0x000F;
                    flag = 0;
                    if ((register[x] >> 7) == 1)
                    {
                        flag = 1;
                    }
                    register[x] *= 2;
                    register[0xF] = flag;
                    break;
            }
        }
        public void case0x9(ushort opcode)
        {
            int Vx;
            int Vy;
            Vx = register[(opcode >> 8) & 0x000F];
            Vy = register[(opcode >> 4) & 0x000F];
            if (Vx != Vy)
            {
                pc += 2;
            }
        }
        public void case0xC(ushort opcode)
        {
            int x;
            int kk;
            x = (opcode >> 8) & 0x000F;
            kk = opcode & 0x00FF;
            int rand = rng.Next(0, 256);
            register[x] = (byte)(kk & rand);
        }
        public void case0xD(ushort opcode)
        {
            int Vx = register[(opcode >> 8) & 0x000F] % 64;
            int Vy = register[(opcode >> 4) & 0x000F] % 32;
            int x;
            int y;
            register[0xF] = 0;

            for (int i = 0; i < (opcode & 0x000F); i++)
            {
                byte sprite = memory[indexRegister + i];
                for (int j = 0; j < 8; j++)
                {
                    if ((sprite & (0x80 >> j)) != 0)
                    {
                        x = (Vx + j) % 64;
                        y = (Vy + i) % 32;

                        if (display[x, y])
                        {
                            register[0xF] = 1;
                        }

                        display[x, y] ^= true;
                    }
                }
            }
        }
        public void case0xE(ushort opcode)
        {
            int x;
            int Vx;
            switch (opcode & 0x00FF)
            {
                case 0x9E:
                    //0xEx9E, if the key Vx is pressed increment pc
                    x = (opcode >> 8) & 0x000F;
                    Vx = register[x];
                    if (key[Vx])
                    {
                        pc += 2;
                    }
                    break;
                case 0xA1:
                    //0xExA1, if the key Vx is not pressed increment pc
                    x = (opcode >> 8) & 0x000F;
                    Vx = register[x];
                    if (!key[Vx])
                    {
                        pc += 2;
                    }
                    break;
            }
        }
        public void case0xF(ushort opcode)
        {
            int x;
            int y;
            switch (opcode & 0x00FF)
            {
                case 0x07:
                    //0xFx07, stores the DelayTimer value in the Vx register
                    register[(opcode >> 8) & 0x000F] = delayTimer;
                    break;
                case 0x0A:
                    //0xFx0A, waits for key press and stores it in Vx
                    x = (byte)((opcode >> 8) & 0x000F);
                    bool check = false;
                    for (int i = 0; i < key.Length; i++)
                    {
                        if (key[i])
                        {
                            check = true;
                            register[x] = (byte)i;
                            break;
                        }
                    }
                    if (!check)
                    {
                        pc -= 2;
                    }
                    break;
                case 0x15:
                    //0xFx15, sets the DelayTimer to the value of Vx
                    delayTimer = register[(opcode >> 8) & 0x000F];
                    break;
                case 0x18:
                    //0xFx18, sets the sound timer to the value of Vx
                    soundTimer = register[(opcode >> 8) & 0x000F];
                    break;
                case 0x1E:
                    //0xFx1E
                    x = (opcode >> 8) & 0x000F;
                    indexRegister += register[x];
                    break;
                case 0x29:
                    //0xFx29, sets the indexRegister to the index of the character Vx
                    byte digit = register[(byte)((opcode >> 8) & 0x000F)];
                    digit *= 5;
                    indexRegister = digit;
                    break;
                case 0x33:
                    //Store BCD representation of Vx in memory locations I, I+1, and I+
                    int bcd = register[(byte)(opcode >> 8) & 0x000F];
                    memory[indexRegister] = (byte)(bcd / 100);
                    memory[indexRegister + 1] = (byte)(bcd % 100 / 10);
                    memory[indexRegister + 2] = (byte)(bcd % 100 % 10);
                    break;
                case 0x55:
                    //0xFx55, stores registers V0 through Vx starting at memory starting at indexRegister
                    x = (byte)((opcode >> 8) & 0x000F);
                    for (int i = 0; i <= x; i++)
                    {
                        memory[indexRegister + i] = register[i];
                    }
                    break;
                case 0x65:
                    //0xFx65, reads registers V0 through Vx to memory starting at indexRegister
                    x = (byte)((opcode >> 8) & 0x000F);
                    for (int i = 0; i <= x; i++)
                    {
                        register[i] = memory[indexRegister + i];
                    }
                    break;
            }
        }
    }
}

using System.Collections;
using System.Text;

namespace Code_Hamming
{
    internal class Program
    {
        static void PrintBits(BitArray bits, int n)
        {
            for(int i=0;i<bits.Length/n;i++)
            {
                for(int j=0;j<n;j++)
                {
                    if(bits[n * i + j])
                        Console.Write('1');
                    else
                        Console.Write('0');
                }
                Console.Write(' ');
            }
            Console.WriteLine();
        }
        static void PrintBytes(byte[] bytes)
        {
            for(int i=0;i<bytes.Length;i++)
            {
                Console.Write(bytes[i]);
                Console.Write(' ');
            }
            Console.WriteLine();
        }

        static BitArray code_block(BitArray bits)
        {
            //create generate matrix
            byte[,] generate_matrix = new byte[,]{{1,0,0,0,1,0,1 },
                                                  {0,1,0,0,1,1,1 },
                                                  {0,0,1,0,1,1,0 },
                                                  {0,0,0,1,0,1,1 }};
            //create bits value
            byte[] bits_v = new byte[4];
            for(int i=0;i<4;i++)
            {
                if (bits[i])
                {
                    bits_v[i] = 1;
                }
                else
                {
                    bits_v[i] = 0;
                }
            }
            //code
            BitArray code_bits = new BitArray(7);
            code_bits.SetAll(false);
            for(int i=0;i<7;i++)
            {
                if((bits_v[0] * generate_matrix[0,i] + bits_v[1] * generate_matrix[1, i] + bits_v[2] * generate_matrix[2, i] + bits_v[3] * generate_matrix[3, i])%2 == 1)
                {
                    code_bits[i] = true;
                }
                else
                {
                    code_bits[i] = false;
                }
            }
            return code_bits;
        }
        static BitArray dcode_block(BitArray bits)
        {
            byte[,] generate_matrix = new byte[,]{{1,0,1},
                                                  {1,1,1},
                                                  {1,1,0},
                                                  {0,1,1},
                                                  {1,0,0},
                                                  {0,1,0},
                                                  {0,0,1}};
            //create bits value
            byte[] bits_v = new byte[7];
            for (int i = 0; i < 7; i++)
            {
                if (bits[i])
                {
                    bits_v[i] = 1;
                }
                else
                {
                    bits_v[i] = 0;
                }
            }
            //dcode
            bool o = true;
            BitArray code_bits = new BitArray(4);
            byte[] S_bits = new byte[3];
            code_bits.SetAll(false);
            for (int i=0;i<4;i++)
            {
                code_bits[i] = bits[i];
            }
            int sum = 0;
            while (o)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        sum += bits_v[j] * generate_matrix[j, i];
                    }
                    if (sum % 2 == 1)
                    {
                        S_bits[i] = 1;
                    }
                    else
                    {
                        S_bits[i] = 0;
                    }
                    sum = 0;
                }
                //check
                o = false;
                for (int i = 0; i < 3; i++)
                {
                    if (S_bits[i] != 0)
                    {
                        o = true;
                    }
                }
                int num = 0;
                if (o)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        if ((S_bits[0] == generate_matrix[i, 0]) && (S_bits[1] == generate_matrix[i, 1]) && (S_bits[2] == generate_matrix[i, 2]))
                        {
                            num = i;
                            break;
                        }
                    }
                    if (num < 4)
                    {
                        code_bits[num] = !code_bits[num];
                        o = false;
                    }
                    else
                    {
                        if (bits_v[num] == 1)
                        {
                            bits_v[num] = 0;
                        }
                        else
                        {
                            bits_v[num] = 1;
                        }
                    }
                }
            }
            return code_bits;
        }
        static BitArray StringToBits(string str)
        {
            BitArray bits = new BitArray(str.Length);
            for(int i=0;i<str.Length;i++)
            {
                if (str[i]=='0')
                {
                    bits[i] = false;
                }
                else
                {
                    bits[i] = true;
                }
            }
            return bits;
        }
        static void Main(string[] args)
        {
            // get mess
            Console.WriteLine("Введите кодируемое сообщение");
            string str = Console.ReadLine();
            //to bits
            byte[] str_bytes = Encoding.ASCII.GetBytes(str);
            BitArray str_bits = new BitArray(str_bytes);
            //print
            Console.Write("Сообщение в байтах: ");
            PrintBytes(str_bytes);
            Console.Write("Сообщение в битах: ");
            PrintBits(str_bits, 8);
            PrintBits(str_bits,4);
            //code
            int n = 4, nr = 7;
            BitArray bits4 = new BitArray(n);
            BitArray bits7 = new BitArray(nr);
            BitArray str_bits_code = new BitArray(str_bits.Length / n * nr);
            for(int i=0;i<str_bits.Length/n;i++)
            {
                bits4.SetAll(false);
                for(int j=0;j<n;j++)
                {
                    bits4[j] = str_bits[i * n + j];
                }
                bits7 = code_block(bits4);
                for (int j = 0; j < nr; j++)
                {
                    str_bits_code[i * nr + j] = bits7[j];
                }
            }
            Console.WriteLine("Закодированное");
            Console.Write("Сообщение в битах: ");
            PrintBits(str_bits_code, nr);
            //read
            Console.WriteLine("Введите ваш код:");
            str_bits_code = StringToBits(Console.ReadLine());
            //decode
            bits4.SetAll(false);
            bits7.SetAll(false);
            str_bits.SetAll(false);
            for (int i=0;i< str_bits_code.Length/nr; i++)
            {
                bits7.SetAll(false);
                for (int j = 0; j < nr; j++)
                {
                    bits7[j] = str_bits_code[i * nr + j];
                }
                bits4 = dcode_block(bits7);
                for (int j = 0; j < n; j++)
                {
                    str_bits[i * n + j] = bits4[j];
                }
            }
            Console.WriteLine("Декодированное");
            Console.Write("Сообщение в битах: ");
            PrintBits(str_bits, n);
            //to bytes
            for(int i=0;i<str_bits.Length/8;i++)
            {
                str_bits.CopyTo(str_bytes, 0);
            }
            Console.Write("Сообщение в байтах: ");
            PrintBytes(str_bytes);
            //to string
            Console.Write("Сообщение: ");
            Console.WriteLine(System.Text.Encoding.ASCII.GetString(str_bytes));
            Console.ReadKey();
        }
    }
}
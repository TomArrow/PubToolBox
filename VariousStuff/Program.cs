using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VariousStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            //quake3Thing();
            //asciiNumbersStuff();
            namehacking();
        }



        static void namehacking()
           {
            const int stringLength = 30;
            Random rnd = new Random();
            byte[] text = new byte[stringLength];
            byte[] textNoColors = new byte[stringLength];
            byte[] textFiltered = new byte[stringLength];
            byte[] textFilteredNoColors = new byte[stringLength];

            StreamWriter log = File.AppendText("namehacking.log");
            for (int i=0; i < 1000000000; i++)
            {
                rnd.NextBytes(text);

                // No zeros
                for(int b = 0; b < stringLength; b++)
                {
                    while(text[b] == 0 || text[b] < 64 || text[b] >= 127 )
                    {
                        text[b] = (byte)rnd.Next(0,255);
                    } 
                }

                // filter
                textFiltered = new byte[stringLength];
                for (int b = 0, f = 0; b < stringLength; b++,f++)
                {
                    
                    if(text[b] != '^'
                        || (b< stringLength - 1 && text[b+1] == '^')
                        || (b>0 && text[b-1] == '^')
                        )
                    {
                        
                    } else
                    {
                        b += 2;
                    }
                    if (b > stringLength - 1) break;
                    textFiltered[f] = text[b];
                }

                // Decolorize text
                // Basically same as above but correctly done.
                textNoColors = new byte[stringLength];
                int textNoColorsLength = 0;
                for (int b = 0; b < stringLength; b++)
                {
                    if (text[b] == 0) break;
                    if (text[b] != '^'
                        || (b < stringLength - 1 && text[b + 1] == '^')
                        || (b > 0 && text[b - 1] == '^')
                        )
                    {

                        textNoColors[textNoColorsLength++] = text[b];
                    }
                    else
                    {
                        b += 1;
                    }
                    if (b > stringLength - 1) break;
                }
                
                textFilteredNoColors = new byte[stringLength];
                int textFilteredNoColorsLength = 0;
                for (int b = 0; b < stringLength; b++)
                {
                    if (textFiltered[b] == 0) break;
                    if (textFiltered[b] != '^'
                        || (b < stringLength - 1 && textFiltered[b + 1] == '^')
                        || (b > 0 && textFiltered[b - 1] == '^')
                        )
                    {

                        textFilteredNoColors[textFilteredNoColorsLength++] = textFiltered[b];
                    }
                    else
                    {
                        b += 1;
                    }
                    if (b > stringLength-1) break;
                }

                if(textNoColorsLength != textFilteredNoColorsLength)
                {
                    string strText = Encoding.ASCII.GetString(text);
                    string strTextFiltered = Encoding.ASCII.GetString(textFiltered);
                    string strTextNoColors = Encoding.ASCII.GetString(textNoColors);
                    string strTextFilteredNoColors = Encoding.ASCII.GetString(textFilteredNoColors);
                    string blah = $"Found hack: \n{strText}\n{strTextFiltered}\n{strTextNoColors}\n{strTextFilteredNoColors}";
                    Console.WriteLine(blah);
                    log.WriteLine(blah);
                }

            }
        }

        static void asciiNumbersStuff()
        {
            List<byte> blah = new List<byte>();
            for(byte i = 160; i < 255; i++)
            {
                blah.Add(i);
            }
            File.WriteAllBytes("asciistufftest.txt", blah.ToArray());
        }


        struct SpeedPositionCombo
        {
            public float speed;
            public float position;
        }

        const float GRAVITY = 800;
        const float JUMP_VELOCITY = 225;//270;
        static void quake3Thing()
        {

            File.AppendAllText("jumptests.csv","frametime,fps,max_jump_height,frames_in_air,seconds_in_air\n");
            for(int frametimeI = 1; frametimeI < 1000; frametimeI++)
            {
                float frametime = (float)frametimeI / 1000.0f;
                float speed = JUMP_VELOCITY;
                float position = 0;
                List<SpeedPositionCombo> data = new List<SpeedPositionCombo>();
                float highestPos = 0;
                int framesInAir = 0;
                while (position>=0)
                {
                    float newSpeed = speed - GRAVITY * frametime;
                    position += 0.5f * (speed + newSpeed) * frametime;
                    //newSpeed = (float)Math.Floor(newSpeed);
                    newSpeed = (float)Math.Round(newSpeed);
                    speed = newSpeed;
                    SpeedPositionCombo dataHere = new SpeedPositionCombo();
                    dataHere.speed = speed;
                    dataHere.position = position;
                    data.Add(dataHere);
                    framesInAir++;
                    highestPos = Math.Max(position, highestPos);
                }
                File.AppendAllText("jumptests.csv", frametimeI+","+1.0f/(float)frametime+","+highestPos+","+framesInAir+","+(framesInAir*frametime)+"\n");
            }
        }
    }
}

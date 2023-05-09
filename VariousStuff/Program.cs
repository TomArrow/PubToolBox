using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace VariousStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string file in args)
            {
                //DoProcess(file);
                //DoProcess2(file);
                DoProcess3(file);
            }

            //DoProcess("dlmeta 2021-09-06.7z");

            Console.ReadKey();
        }
        
        static void MainOld(string[] args)
        {
            //quake3Thing();
            //asciiNumbersStuff();
            namehacking();
        }

        static Regex cases = new Regex(@"case\s*([\dxa-f]+)\s*:(?:\s*?\/\/\s*([^\n\r\s\?]+)([^\n\r]+)?)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex hexnum = new Regex(@"^([\da-fA-F]+)h$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static Regex gitemfinder = new Regex(@"gitem_t(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|int).)*?int\s*([0-9a-fA-Fh]+)(?:(?!gitem_t|itemType_t).)*?itemType_t\s*(IT_\w+)(?:(?!gitem_t|int).)*?int\s*([0-9a-fA-Fh]+)(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?(?:(?!gitem_t|char).)*?char \*[^\n\r=]*(?:=\s*([^\n]+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);



        static void DoProcess3(string filename)
        {
            string text = File.ReadAllText(filename);
            var matches = gitemfinder.Matches(text);
            int maxIndex = 0;
            List<string> items = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach(Match match in matches)
            {
                //sb.Clear();
                sb.Append("{\n");
                for(int i = 1; i < 16; i++)
                {
                    sb.Append("\t");
                    if (i == 3) {
                        sb.Append("{\n");
                        continue;
                    }
                    if(i>=4 && i <= 7)
                    {
                        sb.Append("\t");
                    }
                    if (match.Groups[i].Success)
                    {
                        string val = match.Groups[i].Value.Trim();
                        if (hexnum.Match(val).Success)
                        {
                            int number = Convert.ToInt32($"0x{val.Substring(0,val.Length-1)}",16);
                            sb.Append(number);
                        }
                        else
                        {
                            sb.Append(val);
                        }
                    }
                    else
                    {
                        if (i >= 13)
                        {
                            sb.Append("\"\"");
                        }
                        else
                        {
                            sb.Append("NULL");
                        }
                    }
                    if (i == 7)
                    {
                        sb.Append("\n\t}");
                    }

                    if(i < 15)
                    {
                        sb.Append(",\n");
                    }
                }
                sb.Append("\n},\n");
                items.Add(sb.ToString());
            }

            Console.WriteLine(sb.ToString());
            File.WriteAllText(filename+".gitemparsed.c",sb.ToString());
        }
        static void DoProcess2(string filename)
        {
            string text = File.ReadAllText(filename);
            var matches =  cases.Matches(text);
            int maxIndex = 0;
            Dictionary<int, string> list = new Dictionary<int, string>();
            Dictionary<int, string> listComments = new Dictionary<int, string>();
            foreach(Match match in matches)
            {
                string numberString = match.Groups[1].Value;
                string nameString = match.Groups[2].Success ? match.Groups[2].Value : null;
                string commentString = match.Groups[3].Success ? match.Groups[3].Value : null;
                int parsedNumber = 0;
                if (numberString.StartsWith("0x"))
                {
                    parsedNumber = Convert.ToInt32(numberString, 16);
                }
                else
                {
                    parsedNumber = int.Parse(numberString);
                }
                if (list.ContainsKey(parsedNumber))
                {
                    if(list[parsedNumber] == null)
                    {
                        list[parsedNumber] = nameString;
                    }
                    else
                    {
                        list[parsedNumber] = list[parsedNumber] + "," + nameString;
                    }
                }
                else
                {
                    list.Add(parsedNumber, nameString);
                }
                if (listComments.ContainsKey(parsedNumber))
                {
                    if(listComments[parsedNumber] == null)
                    {
                        listComments[parsedNumber] = commentString;
                    }
                    else
                    {
                        listComments[parsedNumber] = listComments[parsedNumber] + "," + commentString;
                    }
                }
                else
                {
                    listComments.Add(parsedNumber, commentString);
                }
                maxIndex = Math.Max(parsedNumber, parsedNumber);
            }
            StringBuilder sb = new StringBuilder();

            int unknown = 0;
            for(int i = 0; i <= maxIndex; i++)
            {
                if (list.ContainsKey(i) && list[i] != null)
                {
                    sb.Append(list[i]+",");
                }
                else
                {
                    sb.Append($"EV_MBII_UNKNOWN{unknown++},");
                }
                if (listComments.ContainsKey(i) && listComments[i] != null)
                {
                    sb.Append("\t\t\t//"+listComments[i]);
                }
                sb.Append("\n");
            }

            File.WriteAllText(filename+".parsed.c",sb.ToString());
        }

        static void DoProcess(string filename)
        {

            try
            {

                string data = File.ReadAllText(filename);

                JsonSerializerOptions opt = new JsonSerializerOptions();
                opt.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
                WarcupData warcupData = JsonSerializer.Deserialize<WarcupData>(data, opt);
                string cupIdStr = warcupData.cupId;
                List<string> demoPaths = new List<string>();
                foreach (Entry entry in warcupData.vq3Results.valid)
                {
                    demoPaths.Add(entry.demopath);
                }
                foreach (Entry entry in warcupData.vq3Results.invalid)
                {
                    demoPaths.Add(entry.demopath);
                }
                foreach (Entry entry in warcupData.cpmResults.valid)
                {
                    demoPaths.Add(entry.demopath);
                }
                foreach (Entry entry in warcupData.cpmResults.invalid)
                {
                    demoPaths.Add(entry.demopath);
                }

                List<string> demoUrls = new List<string>();
                foreach (string demoPath in demoPaths)
                {
                    //demoUrls.Add("dm_68,https://dfcomps.ru/api/uploads/demos/cup"+cupIdStr+"/"+demoPath);
                }
                //demoUrls.Add("zip,https://dfcomps.ru/api/" + warcupData.cup.archiveLink);


                try
                {
                    TableJson tableData = JsonSerializer.Deserialize<TableJson>(warcupData.tableJson, opt);
                    foreach(KeyValuePair<string,TableEntry> te in tableData.vq3)
                    {
                        demoUrls.Add(HTML5DecodePortable.Utility.HtmlDecode(HttpUtility.HtmlDecode(te.Value.demo)));
                    }
                }
                catch(Exception e)
                {


                }

                File.AppendAllLines("demoUrls2.txt",demoUrls);
                //File.AppendAllLines("tablejson.txt",new string[] { warcupData.tableJson });

            }catch(Exception e)
            {

            }
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

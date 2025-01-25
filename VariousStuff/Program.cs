using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace VariousStuff
{
    class Program
    {
        static void Main(string[] args)
        {
            MainOld(args);
            return;
            foreach (string file in args)
            {
                //DoProcess(file);
                //DoProcess2(file);
                //DoProcess3(file);
            }

            //DoProcess("dlmeta 2021-09-06.7z");

            Console.ReadKey();
        }
        
        static void MainOld(string[] args)
        {
            //quake3Thing();
            //asciiNumbersStuff();
            //namehacking();
            //quake3ThingGravityEquivalent();
            //quake3ThingGravityRatiosOverTime();
            //Q3FPSAcceleration();
            //Q3FPSAccelerationProgressive();
            //Q3FPSAcceleration3D();
            //GroundFrictionMax3D();
            Q3WishSpeedAccelForwardCalc();

        }

        static void GroundFrictionMax3D()
        {
            float accel = 10.0f;
            float friction = 6.0f;
            float basespeed = 250.0f;
            for (int i = 1; i < 1000; i++)
            {
                float maxSpeedFloat = 0;
                float maxSpeedSnap = 0;
                for(int snap =0;snap < 2; snap++)
                {
                    float maxSpeed = 0;
                    float frameTime = (float)i * 0.001f;
                    //float speed = 0;
                    Vector3 speed = new Vector3() { X = 0.001f };
                    for (int a = 0; a < 10000; a++)
                    {
                        speed *= 1.0f - frameTime * friction;
                        float bestAngle = (float)Math.Acos(((basespeed - (accel * basespeed * frameTime)) / speed.Length()));// *(180.0f / (float)Math.PI);
                        if (float.IsNaN(bestAngle))
                        {
                            bestAngle = 0;
                            speed += Vector3.Normalize(speed) * accel * basespeed * frameTime;
                            //speed += accel * basespeed * frameTime;
                        }
                        else
                        {
                            //Vector3 old = new Vector3() { X = speed };
                            //Vector3 add = new Vector3() { X = accel * basespeed * frameTime };
                            Vector3 add = Vector3.Normalize(speed) * accel * basespeed * frameTime;
                            Vector3 rotated = Vector3.Transform(add, Matrix4x4.CreateRotationZ(bestAngle));
                            speed = (speed + rotated);//.Length();
                                                      //speed += accel * basespeed * frameTime;
                            if(snap > 0)
                            {
                                speed.X = (float)Math.Round(speed.X);
                                speed.Y = (float)Math.Round(speed.Y);
                                speed.Z = (float)Math.Round(speed.Z);
                            }
                        }
                        if (speed.Length() > maxSpeed)
                        {
                            maxSpeed = speed.Length();
                        }
                    }

                    if (snap > 0)
                    {
                        maxSpeedSnap = maxSpeed;
                    }
                    else
                    {
                        maxSpeedFloat = maxSpeed;
                    }

                }
                Console.WriteLine($"FPS: {1000.0f / (float)i}, maxSpeed (float): {maxSpeedFloat}, maxSpeed (vsnap): {maxSpeedSnap}");

            }
        }
        static void GroundFrictionMax()
        {
            float accel = 10.0f;
            float friction = 6.0f;
            float basespeed = 250.0f;
            for(int i = 1; i < 1000; i++)
            {
                float maxSpeed = 0;
                float frameTime = (float)i * 0.001f;
                float speed = 0;
                for(int a = 0; a < 10000; a++)
                {
                    speed *= 1.0f - frameTime * friction;
                    float bestAngle = (float)Math.Acos(((basespeed - (accel * basespeed * frameTime)) / speed));// *(180.0f / (float)Math.PI);
                    if (float.IsNaN(bestAngle))
                    {
                        bestAngle = 0; 
                        speed += accel * basespeed * frameTime;
                    }
                    else
                    {
                        Vector3 old = new Vector3() { X = speed };
                        Vector3 add = new Vector3() { X = accel * basespeed * frameTime };
                        Vector3 rotated = Vector3.Transform(add, Matrix4x4.CreateRotationZ(bestAngle));
                        speed = (old + rotated).Length();
                        //speed += accel * basespeed * frameTime;
                    }
                    if (speed > maxSpeed)
                    {
                        maxSpeed = speed;
                    }
                }
                Console.WriteLine($"FPS: {1000.0f/(float)i}, maxSpeed: {maxSpeed}");
            }
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
        static void quake3ThingGravityEquivalent()
        {

            File.AppendAllText("gravityEquivalents.csv", "frametime,fps,default_gravity,effective_gravity,gravityRatio\n");
            for(int frametimeI = 1; frametimeI < 1000; frametimeI++)
            {
                float frametime = (float)frametimeI / 1000.0f;
                float speed = 0;
                float position = 0;
                List<SpeedPositionCombo> data = new List<SpeedPositionCombo>();
                float highestPos = 0;
                float timeElapsed = 0;
                while (timeElapsed<10)
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
                    timeElapsed+= frametime;
                    highestPos = Math.Max(position, highestPos);
                }
                float defaultGravityPos = -GRAVITY * timeElapsed * timeElapsed * 0.5f;
                float gravityRatio = position / defaultGravityPos;
                float effectiveGravity = GRAVITY * gravityRatio;
                File.AppendAllText("gravityEquivalents.csv", $"{frametimeI},{1.0f / (float)frametime},{GRAVITY},{effectiveGravity},{gravityRatio}\n");
            }
        }

        static void quake3ThingGravityRatiosOverTime()
        {

            int[] fpsList = new int[] { 333,250,142,125,83,76,31,30 };
            StringBuilder sb = new StringBuilder();
            sb.Append("time");
            foreach (int fps in fpsList)
            {
                sb.Append($",averageGravityRatio{fps}");
            }
            foreach (int fps in fpsList)
            {
                sb.Append($",localGravityRatio{fps}");
            }
            File.AppendAllText("gravityRatiosOverTime.csv",$"{sb.ToString()}\n");
            sb.Clear();

            List<Quake3FallAnalysisBucket.Sample>[] samples = new List<Quake3FallAnalysisBucket.Sample>[fpsList.Length];

            for (int i = 0; i < fpsList.Length; i++)
            {
                Quake3FallAnalysisBucket bucket = new Quake3FallAnalysisBucket(1000/fpsList[i],0);
                samples[i] = new List<Quake3FallAnalysisBucket.Sample>();
                samples[i].Add(bucket.getSample());
                while (samples[i].Count == 1 || samples[i][samples[i].Count-1].time < 10.0f)
                {
                    bucket.Iterate();
                    samples[i].Add(bucket.getSample());
                }
            }

            int[] sampleIndizi = new int[fpsList.Length];

            float currentTime = 0.0f;
            bool anyIndexIncreased = true;
            while (anyIndexIncreased)
            {
                anyIndexIncreased = false;
                // Make sure each sample array's position is at a time is higher than current.
                // Then we can simply always use the index minus 1 for the currently valid value.
                float nextHighestTime = currentTime;
                for (int i= 0;i < fpsList.Length;i++)
                {
                    while (samples[i][sampleIndizi[i]].time <= currentTime && sampleIndizi[i] < (samples[i].Count-1))
                    {
                        sampleIndizi[i]++;
                        anyIndexIncreased = true;
                    }
                    if ((samples[i][sampleIndizi[i]].time < nextHighestTime && samples[i][sampleIndizi[i]].time > currentTime) || nextHighestTime == currentTime)
                    {
                        nextHighestTime = samples[i][sampleIndizi[i]].time;
                    }
                }

                sb.Clear();
                sb.Append(currentTime);
                for (int i = 0; i < fpsList.Length; i++)
                {
                    Quake3FallAnalysisBucket.Sample sampleHere = samples[i][sampleIndizi[i]-1];
                    sb.Append($",{sampleHere.averageGravityRatio}");
                }
                for (int i = 0; i < fpsList.Length; i++)
                {
                    Quake3FallAnalysisBucket.Sample sampleHere = samples[i][sampleIndizi[i]-1];
                    sb.Append($",{sampleHere.localGravityRatio}");
                }

                currentTime = nextHighestTime;
                File.AppendAllText("gravityRatiosOverTime.csv", $"{sb.ToString()}\n");
            }

            
        }

        static double Q3OptimumAngleCalc(Vector2 vel, AccelSettings set, float friction, float frametime)
        {
            return Math.Acos((double)((set.wishspeed - (set.accel * set.wishspeed * frametime)) / (vel.Length() * (1 - friction * (frametime))))) *180.0/Math.PI;
        }

        //static float AngleSubtract(float angle1,float angle2)
        //{
        //    float angleDelta = angle1 - angle2;
        //    while (angleDelta > 180.0f)
        //    {
        //        angleDelta -= 360.0f;
        //    }
        //    while (angleDelta < -180.0f)
        //    {
        //        angleDelta += 360.0f;
        //    }
        //    return angleDelta;
        //}

        static Vector2 PM_QuaJKAccelerate(Vector2 velocity,Vector2 wishdir, float frametime, float wishspeed, float baseAccel, float maxAccel, float maxAccelWishSpeed)
        {
            // q2 style
            int i;
            float addspeed, accelspeed, currentspeed;
            float accel;
            float f, finalWishSpeed;
            float accelAddSlow, accelAddHigh;
            float neededSpeedSlow, neededSpeedHigh;

            currentspeed = Vector2.Dot(velocity, wishdir);

            if (currentspeed >= wishspeed) return velocity;

            accelAddSlow = baseAccel * frametime * wishspeed;
            accelAddHigh = maxAccel * frametime * maxAccelWishSpeed;

            neededSpeedSlow = wishspeed - accelAddSlow;
            neededSpeedHigh = maxAccelWishSpeed - accelAddHigh;

            f = (currentspeed - neededSpeedHigh) / (neededSpeedSlow - neededSpeedHigh);

            if (f < 0) f = 0;
            else if (f > 1) f = 1;

            accel = (f * baseAccel) + ((1.0f - f) * maxAccel);
            finalWishSpeed = (f * wishspeed) + ((1.0f - f) * maxAccelWishSpeed);

            accelspeed = accel * frametime * finalWishSpeed;

            addspeed = finalWishSpeed - currentspeed;
            if (addspeed <= 0)
            {
                return velocity;
            }

            if (accelspeed > addspeed)
            {
                accelspeed = addspeed;
            }
            velocity.X += accelspeed * wishdir.X;
            velocity.Y += accelspeed * wishdir.Y;
            return velocity;
        }
        static Vector2 PM_DreamAccelerate(Vector2 velocity,Vector2 wishdir, float frametime, float wishspeed, float baseAccel, float maxAccel, float maxAccelWishSpeed)
        {
            // q2 style
            int i;
            float addspeed, accelspeed, currentspeed;
            float accel;
            float f, finalWishSpeed;
            float accelAddSlow, accelAddHigh;
            float neededSpeedSlow, neededSpeedHigh;
            float idealVelRatio;
            Vector2 idealFrontalVec;
            float frontOvershoot;

            currentspeed = Vector2.Dot(velocity, wishdir);

            if (currentspeed >= wishspeed) return velocity;

            accelAddSlow = baseAccel * frametime * wishspeed;
            accelAddHigh = maxAccel * frametime * maxAccelWishSpeed;

            neededSpeedSlow = wishspeed - accelAddSlow;
            neededSpeedHigh = maxAccelWishSpeed - accelAddHigh;

            f = (currentspeed - neededSpeedHigh) / (neededSpeedSlow - neededSpeedHigh);

            if (f < 0) f = 0;
            else if (f > 1) f = 1;

            // f =1.0f- (1.0f-f) * (1.0f-f);
            //f =0.94f*f + 0.06f*f*f*f;

            accel = (f * baseAccel) + ((1.0f - f) * maxAccel);
            finalWishSpeed = (f * wishspeed) + ((1.0f - f) * maxAccelWishSpeed);

            accelspeed = accel * frametime * finalWishSpeed;

            addspeed = finalWishSpeed - currentspeed;
            if (addspeed <= 0)
            {
                return velocity;
            }

            if (accelspeed > addspeed)
            {
                accelspeed = addspeed;
            }

            wishdir *= accelspeed;

            float velTotal =velocity.Length();
            float w = accelAddSlow + wishspeed;
            idealVelRatio = w * w / (velocity.LengthSquared());
            idealVelRatio *= accelAddSlow / (wishspeed+accelAddSlow);
            float maxFront = idealVelRatio * velTotal;
            Vector2 normVel = Vector2.Normalize(velocity);
            //idealFrontalVec = velocity * idealVelRatio;
            //frontOvershoot = (velocity + wishdir * 2).Length() - velTotal;
            //frontOvershoot = (float)Math.Sqrt(velTotal*velTotal+accelspeed*accelspeed*2) - velTotal;
            //frontOvershoot = Vector2.Dot(wishdir, normVel);
            while(((velocity + wishdir * 2).Length() - velTotal) > maxFront * 2)
            {
                wishdir *= 0.98f;
            }

            //wishdir *= (1.0f- ((wishspeed + accelAddSlow)-currentspeed)*0.0001f);
            /*if(frontOvershoot > maxFront*2)
            {
                double x = Math.Sqrt(maxFront) * Math.Sqrt(maxFront + velTotal);

                float ratio = (float)x / accelspeed;
                ratio =(float) (ratio);
                wishdir *= ratio;
            }*/

            velocity += wishdir;
            return velocity;
        }

        static Vector2 Q3ApplyOptimumAccel(Vector2 vel, AccelSettings set, float friction, float frametime, Vector2 approach, ref float angleChange)
        {
            double bestAngle = Q3OptimumAngleCalc(vel, set, friction, frametime);
            float originalAngle = Q3Accelerator.vecToYaw(Vector2.Normalize(vel));
            float angleApproach = Q3Accelerator.vecToYaw(Vector2.Normalize(approach));
            float angleDelta = AngleSubtract(originalAngle, angleApproach);
            float angle = originalAngle;
            bestAngle += set.angleOffset;
            angle += angleDelta > 0 ? -(float)bestAngle : (float)bestAngle;
            Vector2 frontVec = Q3Accelerator.AngleFrontVec(angle);
            if (set.quajkMode)
            {
                if (set.dreamMode)
                {
                    frontVec = PM_DreamAccelerate(vel, frontVec, frametime, 320, 1, 30,200);
                }
                else
                {
                    frontVec = PM_QuaJKAccelerate(vel, frontVec, frametime, 320, 1, 70, 30);
                }
            }
            else
            {
                frontVec *= set.accel * frametime * set.wishspeed;
                frontVec += vel;
            }
            float angleNew = Q3Accelerator.vecToYaw(Vector2.Normalize(frontVec));
            float newAngleDelta = Math.Abs(AngleSubtract(angleNew, originalAngle));
            angleChange += newAngleDelta;
            return frontVec;
        }

        struct AccelSettings {
            public string name;
            public float wishspeed;
            public float accel;
            public bool quajkMode;
            public bool dreamMode;
            public float angleOffset;
        }


        static void Q3WishSpeedAccelForwardCalc()
        {
            Vector2 start = new Vector2(400,0);
            Vector2 startNormalized = Vector2.Normalize(start);
            float frametime = 0.007f;
            AccelSettings[] settingsSet = new AccelSettings[] {  
                new AccelSettings(){wishspeed = 320,accel = 1.0f,name="VQ3" }, // vq3
                new AccelSettings(){wishspeed = 30,accel = 70.0f,name="CPM" }, // vq3
                new AccelSettings(){wishspeed = 200,accel = 2f,name="test1" }, // vq3
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=0.0f,name="QuaJK-0" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=1.0f,name="QuaJK-1" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=2.0f,name="QuaJK-2" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=3.0f,name="QuaJK-3" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=4.0f,name="QuaJK-4" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=5.0f,name="QuaJK-5" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=6.0f,name="QuaJK-6" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=7.0f,name="QuaJK-7" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=8.0f,name="QuaJK-8" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=9.0f,name="QuaJK-9" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=10.0f,name="QuaJK-10" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=11.0f,name="QuaJK-11" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=12.0f,name="QuaJK-12" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=13.0f,name="QuaJK-13" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=14.0f,name="QuaJK-14" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=15.0f,name="QuaJK-15" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=16.0f,name="QuaJK-16" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=17.0f,name="QuaJK-17" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=18.0f,name="QuaJK-18" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=19.0f,name="QuaJK-19" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=20.0f,name="QuaJK-20" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=25.0f,name="QuaJK-25" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=30.0f,name="QuaJK-30" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=35.0f,name="QuaJK-35" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=40.0f,name="QuaJK-40" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=45.0f,name="QuaJK-45" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,angleOffset=50.0f,name="QuaJK-50" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=0.0f,name="dream-0" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=1.0f,name="dream-1" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=2.0f,name="dream-2" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=3.0f,name="dream-3" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=4.0f,name="dream-4" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=5.0f,name="dream-5" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=6.0f,name="dream-6" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=7.0f,name="dream-7" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=8.0f,name="dream-8" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=9.0f,name="dream-9" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=10.0f,name="dream-10" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=11.0f,name="dream-11" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=12.0f,name="dream-12" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=13.0f,name="dream-13" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=14.0f,name="dream-14" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=15.0f,name="dream-15" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=16.0f,name="dream-16" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=17.0f,name="dream-17" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=18.0f,name="dream-18" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=19.0f,name="dream-19" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=20.0f,name="dream-20" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=25.0f,name="dream-25" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=30.0f,name="dream-30" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=35.0f,name="dream-35" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=40.0f,name="dream-40" },
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=45.0f,name="dream-45" }, 
                new AccelSettings(){wishspeed = 320,accel = 1.0f,quajkMode=true,dreamMode=true,angleOffset=50.0f,name="dream-50" },
            };

            float[] forwardAccel = new float[settingsSet.Length];
            float[] angleChange = new float[settingsSet.Length];
            float[] totalSpeedGain = new float[settingsSet.Length];

            for(int s=0;s<settingsSet.Length;s++)
            {
                AccelSettings settings = settingsSet[s];
                Vector2 current = start;
                float anglechange = 0;
                float time = 0;
                while (time < 1.0)
                {
                    current = Q3ApplyOptimumAccel(current,settings,0,frametime,start,ref anglechange);
                    time += frametime;
                }
                float delta = current.Length() - start.Length();
                float startDirVel = Vector2.Dot(startNormalized, current);
                float deltaSameDir = startDirVel - start.Length();
                delta /= time;
                deltaSameDir /= time;
                anglechange /= time;
                totalSpeedGain[s] = delta;
                forwardAccel[s] = deltaSameDir;
                angleChange[s] = anglechange;
                Console.WriteLine($"Results for {settings.name}: {delta}, {deltaSameDir}, {anglechange}");
            }
        }

        static void Q3FPSAcceleration()
        {
            int maxBaseVelocity = 4000;
            int[] fpsList = new int[] { 333, 250, 142, 125, 90, 83, 76, 31, 30 };

            StringBuilder sb = new StringBuilder();
            sb.Append("baseVelocity");
            foreach (int fps in fpsList)
            {
                sb.Append($",maxAccel{fps}");
            }
            foreach (int fps in fpsList)
            {
                sb.Append($",avgSameHemisphereAccel{fps}");
            }
            File.AppendAllText("maxAccelFpsV2.csv", $"{sb.ToString()}\n");
            sb.Clear();

            float[,] maxAccelValues = new float[fpsList.Length, maxBaseVelocity];
            float[,] maxAvgSameHemisphereAccelValues = new float[fpsList.Length, maxBaseVelocity];
            Parallel.For(0,fpsList.Length,(int fpsIndex)=> {

                int fps = fpsList[fpsIndex];
                Q3Accelerator accelerator = new Q3Accelerator();
                for(int v=0;v< maxBaseVelocity; v++)
                {
                    int msec = 1000 / fps;
                    float averageSameHemisphereAccel = 0;
                    float maxAccelHere = accelerator.findMaximumAcceleration(msec, v, ref averageSameHemisphereAccel);
                    float normalizedAccel = 1000.0f*maxAccelHere / (float)msec;
                    float normalizedAvgSameHemisphereAccel = 1000.0f* averageSameHemisphereAccel / (float)msec;
                    Console.WriteLine($"{fps},{v}: {normalizedAccel} peak, {normalizedAvgSameHemisphereAccel} avg (same hemisphere)");
                    maxAccelValues[fpsIndex,v] = normalizedAccel;
                    maxAvgSameHemisphereAccelValues[fpsIndex,v] = normalizedAvgSameHemisphereAccel;
                }

            });

            for (int v = 0; v < maxBaseVelocity; v++)
            {
                sb.Append(v);
                for(int f = 0; f < fpsList.Length; f++)
                {
                    sb.Append($",{maxAccelValues[f,v]}");
                }
                for(int f = 0; f < fpsList.Length; f++)
                {
                    sb.Append($",{maxAvgSameHemisphereAccelValues[f,v]}");
                }
                File.AppendAllText("maxAccelFpsV2.csv", $"{sb.ToString()}\n");
                sb.Clear();
            }
            Console.ReadKey();
        }
        static void Q3FPSAcceleration3D()
        {
            int maxBaseVelocity = 4000;
            int[] fpsList = new int[] { 333, 250, 142, 125, 90, 83, 76, 31, 30 };

            StringBuilder sb = new StringBuilder();
            sb.Append("baseVelocity,angle,");
            foreach (int fps in fpsList)
            {
                sb.Append($",maxAccel{fps}");
            }
            foreach (int fps in fpsList)
            {
                sb.Append($",avgSameHemisphereAccel{fps}");
            }
            File.AppendAllText("maxAccelFps3D.csv", $"{sb.ToString()}\n");
            sb.Clear();

            float[,,] maxAccelValues = new float[fpsList.Length, maxBaseVelocity, 360];
            float[,,] maxAvgSameHemisphereAccelValues = new float[fpsList.Length, maxBaseVelocity, 360];
            Parallel.For(0,fpsList.Length,(int fpsIndex)=> {

                int fps = fpsList[fpsIndex];
                Q3Accelerator accelerator = new Q3Accelerator();
                for(int v=0;v< maxBaseVelocity; v++)
                {
                    int msec = 1000 / fps;
                    float averageSameHemisphereAccel = 0;
                    float[] accelsBasedOnAngle = new float[360];
                    float[] accelsAvgSameHemisphereBasedOnAngle = new float[360];
                    float maxAccelHere = accelerator.findMaximumAcceleration(msec, v, ref averageSameHemisphereAccel, accelsBasedOnAngle, 10, accelsAvgSameHemisphereBasedOnAngle);
                    float normalizedAccel = 1000.0f*maxAccelHere / (float)msec;
                    float normalizedAvgSameHemisphereAccel = 1000.0f* averageSameHemisphereAccel / (float)msec;
                    Console.WriteLine($"{fps},{v}: {normalizedAccel} peak, {normalizedAvgSameHemisphereAccel} avg (same hemisphere)");

                    for(int a = 0; a < 360; a++)
                    {
                        float normalizedAccelThisAngle = 1000.0f * accelsBasedOnAngle[a] / (float)msec;
                        maxAccelValues[fpsIndex, v, a] = normalizedAccelThisAngle;
                        float normalizedAccelAvgSameHemisphereThisAngle = 1000.0f * accelsAvgSameHemisphereBasedOnAngle[a] / (float)msec;
                        maxAvgSameHemisphereAccelValues[fpsIndex, v, a] = normalizedAccelAvgSameHemisphereThisAngle;
                    }
                }

            });

            StringBuilder outerSb = new StringBuilder();
            for (int v = 0; v < maxBaseVelocity; v++)
            {
                for (int a = 0; a < 360; a++)
                {
                    sb.Append($"{v},{a}");
                    for (int f = 0; f < fpsList.Length; f++)
                    {
                        sb.Append($",{maxAccelValues[f, v, a]}");
                    }
                    for (int f = 0; f < fpsList.Length; f++)
                    {
                        sb.Append($",{maxAvgSameHemisphereAccelValues[f, v, a]}");
                    }
                    outerSb.Append($"{sb.ToString()}\n");
                    sb.Clear(); 
                }
            }
            File.AppendAllText("maxAccelFps3D.csv", $"{outerSb.ToString()}\n");
            Console.ReadKey();
        }

        static float AngleSubtract(float a1, float a2)
        {
            float a;

            a = a1 - a2;
            while (a > 180)
            {
                a -= 360;
            }
            while (a < -180)
            {
                a += 360;
            }
            return a;
        }

        static void Q3FPSAccelerationProgressive()
        {
            int maxBaseVelocity = 4000;
            int[] fpsList = new int[] { 333, 250, 142, 125, 90, 83, 76, 31, 30 };

            StringBuilder sb = new StringBuilder();
            sb.Append("baseVelocity");
            foreach (int fps in fpsList)
            {
                sb.Append($",maxAccel{fps}");
            }
            File.AppendAllText("maxAccelFpsProgressive.csv", $"{sb.ToString()}\n");
            sb.Clear();

            bool angleChangeConstraints = true;
            bool angleChangeConstrainOnlyAllowChangeWhen0Accel = false;
            bool quick0AccelSkip = false; // Don't use this when plotting over time!
            bool compensateAccelForZeroAccelTimeLost = true;
            int minAngleChangeDelay = 1000;

            float[,] maxAccelValues = new float[fpsList.Length, maxBaseVelocity];
            Parallel.For(0,fpsList.Length, new ParallelOptions() { /*MaxDegreeOfParallelism = 1*/ }, (int fpsIndex)=> {

                int fps = fpsList[fpsIndex];
                Q3Accelerator accelerator = new Q3Accelerator();
                int lastVelocity = 0;
                accelerator.SetVelocityAndAngle(200, 0);
                float accel = 0;
                int msec = 1000 / fps;

                int timePassed = 0;
                int lastAngleChange = -minAngleChangeDelay;

                float lastAngle = 0;
                float lastAngleDiff = 0;

                int lastTimeNot0Accel = 0;

                while (lastVelocity <= maxBaseVelocity)
                {
                    // Reset any angle constraints if we had any if the delay has passed.
                    if (((timePassed - lastAngleChange) >= minAngleChangeDelay && !angleChangeConstrainOnlyAllowChangeWhen0Accel) || accel == 0 && quick0AccelSkip)
                    {
                        accelerator.progressiveConstrainAngleMax = null;
                        accelerator.progressiveConstrainAngleMin = null;
                    }

                    int currentVelocity = (int)Math.Round(accelerator.getVelocity().Length());
                    for (int vel = lastVelocity;vel< Math.Min(currentVelocity,maxBaseVelocity);vel++)
                    {
                        maxAccelValues[fpsIndex, vel] = accel;
                    }
                    lastVelocity = currentVelocity;

                    float chosenAngle = 0;
                    float realAccel = accelerator.findMaximumAccelerationProgressive(msec, ref chosenAngle);
                    float angleDiff = AngleSubtract(chosenAngle,lastAngle);
                    if((angleDiff * lastAngleDiff) < 0 && angleChangeConstraints) // aka: if one is negative and one positive, indicating a direction change
                    {
                        if(angleDiff > 0)
                        {
                            accelerator.progressiveConstrainAngleMin = chosenAngle;
                            accelerator.progressiveConstrainAngleMax = chosenAngle+90;
                        } else
                        {
                            accelerator.progressiveConstrainAngleMin = chosenAngle-90;
                            accelerator.progressiveConstrainAngleMax = chosenAngle;
                        }
                        lastAngleChange = timePassed;
                    } else if(accelerator.progressiveConstrainAngleMin.HasValue && accelerator.progressiveConstrainAngleMax.HasValue && angleDiff != 0)
                    {
                        // Update the constraints so we don't eventually run into a brickwall even though direction hasn't changed.
                        if (angleDiff > 0)
                        {
                            accelerator.progressiveConstrainAngleMin = chosenAngle;
                            accelerator.progressiveConstrainAngleMax = chosenAngle + 90;
                        }
                        else
                        {
                            accelerator.progressiveConstrainAngleMin = chosenAngle - 90;
                            accelerator.progressiveConstrainAngleMax = chosenAngle;
                        }
                    }
                    lastAngle = chosenAngle;
                    lastAngleDiff = angleDiff;
                    float normalizedAccel = 1000.0f * realAccel / (float)(msec + (compensateAccelForZeroAccelTimeLost ? (timePassed - lastTimeNot0Accel):0));
                    Console.WriteLine($"{fps},{currentVelocity}: {normalizedAccel}");
                    accel = normalizedAccel;
                    timePassed += msec;
                    if(realAccel > 0)
                    {
                        lastTimeNot0Accel = timePassed;
                    }
                }
                
            });

            for (int v = 0; v < maxBaseVelocity; v++)
            {
                sb.Append(v);
                for(int f = 0; f < fpsList.Length; f++)
                {
                    sb.Append($",{maxAccelValues[f,v]}");
                }
                File.AppendAllText("maxAccelFpsProgressive.csv", $"{sb.ToString()}\n");
                sb.Clear();
            }
            Console.ReadKey();
        }

        class Quake3FallAnalysisBucket
        {
            public struct Sample
            {
                public float time;
                public float speed;
                public float localGravityRatio;
                public float averageGravityRatio;
            }

            float frametime = 0;
            float speed = 0;
            float position = 0;
            List<SpeedPositionCombo> data = new List<SpeedPositionCombo>();
            float highestPos = 0;
            float timeElapsed = 0;

            float currentGravityRatio = 1.0f;

            public Quake3FallAnalysisBucket(int frametimeI, float startVelocity = 0)
            {
                frametime = (float)frametimeI / 1000.0f;
                speed = startVelocity;
            }

            public void Iterate()
            {
                float oldSpeed = speed;
                float newSpeed = speed - GRAVITY * frametime;
                float newSpeedUnrounded = newSpeed;
                position += 0.5f * (speed + newSpeed) * frametime;
                //newSpeed = (float)Math.Floor(newSpeed);
                newSpeed = (float)Math.Round(newSpeed);
                speed = newSpeed;
                SpeedPositionCombo dataHere = new SpeedPositionCombo();
                dataHere.speed = speed;
                dataHere.position = position;
                data.Add(dataHere);
                timeElapsed += frametime;
                highestPos = Math.Max(position, highestPos);

                currentGravityRatio = (newSpeed - oldSpeed) / (newSpeedUnrounded - oldSpeed);
            }

            public Sample getSample()
            {
                float gravityRatio = 1.0f;
                if(timeElapsed > 0)
                {
                    float defaultGravityPos = -GRAVITY * timeElapsed * timeElapsed * 0.5f;
                    gravityRatio = position / defaultGravityPos;
                    float effectiveGravity = GRAVITY * gravityRatio;
                }
                return new Sample()
                {
                    time = timeElapsed,
                    averageGravityRatio = gravityRatio,
                    localGravityRatio = currentGravityRatio,
                    speed = speed
                };
            }
        }

        class Q3Accelerator
        {
            class usercmd_t
            {
                public int serverTime;
                public int[] angles = new int[3];
                public int buttons;
                public byte weapon;           // weapon 
                public byte forcesel;
                public byte invensel;
                public byte generic_cmd;
                public sbyte forwardmove, rightmove, upmove;
            }

            struct pml_t
            {
                public Vector3 forward;
                public Vector3 right;
                public Vector3 up;
            }

            pml_t pml = new pml_t();
            float currentAccelerationAngle = 0.0f;

            static float pm_airaccelerate = 1.0f;

            Vector3 velocity = new Vector3();
            float frameTime = 0.007f;

            public Vector3 getVelocity()
            {
                return velocity;
            }


            public float findMaximumAcceleration(int msec, float wishVelocity, ref float averageSameHemisphereAccel, float[] accelsBasedOnAngle = null, int outerLoopIncrement = 1, float[] avgHemisphereAccelsBasedOnAngle = null)
            {
                float maxAccel = 0.0f;

                frameTime = (float)msec * 0.001f;

                double accelSum = 0;
                double accelCount = 0;

                // Ugly but whatever. Brute force. We try each angle in 0.1 increments for both current velocity and acceleration.
                for (int i=0;i<3600;i+= outerLoopIncrement)
                {
                    float angleVel = (float)i / 10.0f;
                    SetVelocityAndAngle(wishVelocity, angleVel);
                    SnapVelocity();
                    Vector3 storedVel = velocity;

                    float maxAccelForThisAngle = 0;

                    double accelSumThisAngle = 0;
                    double accelCountThisAngle = 0;

                    for (int j = 0; j < 3600; j++)
                    {
                        float angleACc = (float)j / 10.0f;
                        SetAccelerationAngle(angleACc);
                        AirAccelerate();
                        SnapVelocity();
                        //float acceleration = Vector3.Distance(storedVel, velocity);
                        float acceleration = velocity.Length() - storedVel.Length();
                        if (maxAccel < acceleration && velocity.Length() > storedVel.Length())
                        {
                            maxAccel = acceleration;
                        }
                        if (maxAccelForThisAngle < acceleration && velocity.Length() > storedVel.Length())
                        {
                            maxAccelForThisAngle = acceleration;
                        }
                        if (Math.Abs(AngleSubtract(angleACc, angleVel)) < 90)
                        {
                            accelSum += acceleration;
                            accelCount++;
                            accelSumThisAngle += acceleration;
                            accelCountThisAngle++;
                        }
                        velocity = storedVel;
                    }

                    if ((i % 10) == 0 && accelsBasedOnAngle != null)
                    {
                        accelsBasedOnAngle[i / 10] = maxAccelForThisAngle;
                    }
                    if ((i % 10) == 0 && avgHemisphereAccelsBasedOnAngle != null)
                    {
                        avgHemisphereAccelsBasedOnAngle[i / 10] = (float)(accelSumThisAngle / accelCountThisAngle);
                    }
                }
                averageSameHemisphereAccel = (float)(accelSum / accelCount);
                return maxAccel;
            }

            public float? progressiveConstrainAngleMin = null;
            public float? progressiveConstrainAngleMax = null;

            public bool isAngleConstraintSatisfied(float min, float max, float actual)
            {
                // Normalize all the angles.
                while (actual > 360.0f)
                {
                    actual -= 360.0f;
                }
                while (actual < 0.0f)
                {
                    actual += 360.0f;
                }
                while (min > 360.0f)
                {
                    min -= 360.0f;
                }
                while (min < 0.0f)
                {
                    min += 360.0f;
                }
                while (max > 360.0f)
                {
                    max -= 360.0f;
                }
                while (max < 0.0f)
                {
                    max += 360.0f;
                }

                if(max > min)
                {
                    // Normal, for example min 5, max 15
                    return actual >= min && actual <= max;
                } else if(min > max)
                {
                    // It's always clockwise from min to max.
                    // So min can actually have a bigger value.
                    // For example min 357, max 5
                    // In this case it always wraps around.
                    // Since no angle can be greater than 360 and no angle can be smaller than 0, 
                    // we are satisfied if either actual >= min or actual <= max.
                    return actual >= min || actual <= max;
                } else
                {
                    // min and max identical. Check for being identical.
                    return actual == min;
                }
            }


            // Remember the resulting velocity value and keep working from that after.
            public float findMaximumAccelerationProgressive(int msec, ref float bestAccelerationAngle)
            {
                float maxAccel = 0.0f;

                frameTime = (float)msec * 0.001f;

                Vector3 newVelocity = velocity;

                // Ugly but whatever. Brute force. We try each angle in 0.1 increments for both current velocity and acceleration.

                SnapVelocity();
                Vector3 storedVel = velocity;
                for (int j = 0; j < 3600; j++)
                {
                    float angleACc = (float)j / 10.0f;
                    if (progressiveConstrainAngleMin.HasValue && progressiveConstrainAngleMax.HasValue && !isAngleConstraintSatisfied(progressiveConstrainAngleMin.Value,progressiveConstrainAngleMax.Value,angleACc))
                    {
                        continue;
                    }
                    SetAccelerationAngle(angleACc);
                    AirAccelerate();
                    SnapVelocity();
                    //float acceleration = Vector3.Distance(storedVel, velocity);
                    float acceleration = velocity.Length() - storedVel.Length();
                    if(maxAccel < acceleration && velocity.Length() > storedVel.Length())
                    {
                        maxAccel = acceleration;
                        newVelocity = velocity;
                        bestAccelerationAngle = angleACc;
                    }
                    velocity = storedVel;
                }

                velocity = newVelocity;
                return maxAccel;
            }

            public void SnapVelocity()
            {
                velocity.X = (float)Math.Round(velocity.X);
                velocity.Y = (float)Math.Round(velocity.Y);
                velocity.Z = (float)Math.Round(velocity.Z);
            }

            void AirAccelerate()
            {
                int i;
                Vector3 wishvel;
                float fmove, smove;
                Vector3 wishdir;
                float wishspeed;
                float scale;
                usercmd_t cmd = new usercmd_t();


                cmd.forwardmove = 127;

                fmove = cmd.forwardmove;
                smove = cmd.rightmove;

                scale = PM_CmdScale(ref cmd);

                // project moves down to flat plane
                pml.forward.Z = 0;
                pml.right.Z = 0;
                VectorNormalize(ref pml.forward);
                VectorNormalize(ref pml.right);

                wishvel.X = pml.forward.X * fmove + pml.right.X * smove;
                wishvel.Y = pml.forward.Y * fmove + pml.right.Y * smove;
                wishvel.Z = 0;

                wishdir = wishvel;
                wishspeed = VectorNormalize(ref wishdir);
                wishspeed *= scale;

                // not on ground, so little effect on velocity
                PM_Accelerate(wishdir, wishspeed, pm_airaccelerate);

            }


            void SetAccelerationAngle(float angle)
            {
                Vector3 angles = new Vector3();
                angles.Y = angle;
                AngleVectors(angles,ref pml.forward, ref pml.right, ref pml.up);
                currentAccelerationAngle = angle;
            }
            public void SetVelocityAndAngle(float wishVelocity, float angle)
            {
                Vector3 angles = new Vector3();
                angles.Y = angle;
                Vector3 forward = new Vector3();
                Vector3 right = new Vector3();
                Vector3 up = new Vector3();
                AngleVectors(angles,ref forward, ref right, ref up);
                VectorNormalize(ref forward);
                forward *= wishVelocity;
                velocity = forward;
            }

            float VectorNormalize(ref Vector3 v)
            {
                float length, ilength;

                length = v.X * v.X + v.Y * v.Y + v.Z * v.Z;
                length = (float)Math.Sqrt(length);

                if (length != 0.0f)
                {
                    ilength = 1 / length;
                    v.X *= ilength;
                    v.Y *= ilength;
                    v.Z *= ilength;
                }

                return length;
            }

            float PM_CmdScale(ref usercmd_t cmd, float speed = 250)
            {
                int max;
                float total;
                float scale;
                int umove = 0; //cmd->upmove;
                               //don't factor upmove into scaling speed

                max = Math.Abs(cmd.forwardmove);
                if (Math.Abs(cmd.rightmove) > max)
                {
                    max = Math.Abs(cmd.rightmove);
                }
                if (Math.Abs(umove) > max)
                {
                    max = Math.Abs(umove);
                }
                if (max == 0)
                {
                    return 0;
                }

                total = (float)Math.Sqrt(cmd.forwardmove * cmd.forwardmove
                    + cmd.rightmove * cmd.rightmove + umove * umove);
                scale = (float)(speed * max / (127.0 * total));

                return scale;
            }
            void PM_Accelerate(Vector3 wishdir, float wishspeed, float accel)
            {

	            // q2 style
	            int			i;
	            float		addspeed, accelspeed, currentspeed;

	            currentspeed = DotProduct (velocity, wishdir);
	            addspeed = wishspeed - currentspeed;
	            if (addspeed <= 0) {
		            return;
	            }
	            accelspeed = accel*frameTime*wishspeed;
	            if (accelspeed > addspeed) {
		            accelspeed = addspeed;
	            }

                velocity += accelspeed * wishdir;
                //velocity.X += accelspeed * wishdir.X;
                //velocity.Y += accelspeed * wishdir.Y;
                //velocity.Z += accelspeed * wishdir.Z;
            }

            float DotProduct(Vector3 x,Vector3 y)
            {
                return ((x).X * (y).X + (x).Y * (y).Y + (x).Z * (y).Z);
            }

            const int PITCH = 0;
            const int YAW  = 1;
            const int ROLL = 2;

            public static Vector2 AngleFrontVec(float yaw)
            {
                Vector3 angels = new Vector3() { Y = yaw };
                Vector3 forward = new Vector3(), right= new Vector3(), up= new Vector3();
                AngleVectors(angels, ref forward, ref right, ref up);
                return new Vector2(forward.X, forward.Y);
            }

            public static void AngleVectors(Vector3 angles, ref Vector3 forward, ref Vector3 right, ref Vector3 up)
            {
                float angle;
                float sr, sp, sy, cr, cp, cy;
                // static to help MS compiler fp bugs

                angle = angles.Y * ((float)Math.PI * 2f / 360f);
                sy = (float)Math.Sin(angle);
                cy = (float)Math.Cos(angle);
                angle = angles.X * ((float)Math.PI * 2f / 360f);
                sp = (float)Math.Sin(angle);
                cp = (float)Math.Cos(angle);
                angle = angles.Z * ((float)Math.PI * 2f / 360f);
                sr = (float)Math.Sin(angle);
                cr = (float)Math.Cos(angle);

                forward.X = cp * cy;
                forward.Y = cp * sy;
                forward.Z = -sp;
                right.X = (-1 * sr * sp * cy + -1 * cr * -sy);
                right.Y = (-1 * sr * sp * sy + -1 * cr * cy);
                right.Z = -1 * sr * cp;
                up.X = (cr * sp * cy + -sr * -sy);
                up.Y = (cr * sp * sy + -sr * cy);
                up.Z = cr * cp;
            }

            public static float vecToYaw(Vector2 vec)
            {
                Vector3 full = new Vector3(vec,0);
                Vector3 angles = new Vector3();
                vectoangles(full, ref angles);
                return angles.Y;
            }
            public static void vectoangles(Vector3 value1, ref Vector3 angles)
            {
                float forward;
                float yaw, pitch;

                if (value1.Y == 0 && value1.X == 0)
                {
                    yaw = 0;
                    if (value1.Z > 0)
                    {
                        pitch = 90;
                    }
                    else
                    {
                        pitch = 270;
                    }
                }
                else
                {
                    if (value1.X != 0)
                    {
                        yaw = (float)(Math.Atan2(value1.Y, value1.X) * 180 / Math.PI);
                    }
                    else if (value1.Y > 0)
                    {
                        yaw = 90;
                    }
                    else
                    {
                        yaw = 270;
                    }
                    if (yaw < 0)
                    {
                        yaw += 360;
                    }

                    forward = (float)Math.Sqrt(value1.X * value1.X + value1.Y * value1.Y);
                    pitch = (float)(Math.Atan2(value1.Z, forward) * 180 / Math.PI);
                    if (pitch < 0)
                    {
                        pitch += 360;
                    }
                }


                angles.X = -pitch;
                angles.Y = yaw;
                angles.Z = 0;
            }

        }
    }
}

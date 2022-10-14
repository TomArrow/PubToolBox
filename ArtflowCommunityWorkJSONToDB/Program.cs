using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ArtflowCommunityWorkJSONToDB
{
    class Program
    {
        static void Main(string[] args)
        {

            foreach(string file in args)
            {
                DoProcess(file);
            }

            //DoProcess("dlmeta 2021-09-06.7z");

            Console.ReadKey();
        }

        static void DoProcess(string fileLocation)
        {

            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;


            var db = new SQLiteConnection(fileLocation + ".dataCDX.db");
            db.CreateTable<ArtFlowImageData>();

            db.BeginTransaction();

            Dictionary<string, int> errorCounts = new Dictionary<string, int>();

            byte[] extractData = File.ReadAllBytes(fileLocation);
            using (MemoryStream readStream = new MemoryStream(extractData)) { 
                using (SevenZipArchive archive = SevenZipArchive.Open(readStream)) {
                    using (IReader reader = archive.ExtractAllEntries()){
                        while (reader.MoveToNextEntry()){
                            if (reader.Entry.Size > 0 && !reader.Entry.IsDirectory){
                                using (EntryStream entryStream = reader.OpenEntryStream())
                                {

                                    MemoryStream ms = new MemoryStream();
                                    //extract.BeginExtractFiles()
                                    //entryStream.CopyTo();
                                    entryStream.CopyTo(ms);

                                    string file = Encoding.UTF8.GetString(ms.ToArray());
                                    try
                                    {

                                        ArtFlowImageData[] images = JsonSerializer.Deserialize<ArtFlowImageData[]>(file, opt);

                                        foreach(ArtFlowImageData image in images)
                                        {
                                            db.Insert(image);
                                        }

                                    } catch (Exception e) // In case JSON decoding fails due to corrupt download etc
                                    {
                                        if (errorCounts.ContainsKey(e.Message))
                                        {
                                            errorCounts[e.Message]++;
                                        } else
                                        {
                                            errorCounts.Add(e.Message, 1);
                                        }
                                        //Console.WriteLine(e.Message);
                                    }


                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine("done. Erorrs: ");
            foreach(KeyValuePair<string,int> kp in errorCounts)
            {
                Console.WriteLine(kp.Value +"x: "+kp.Key);
            }

            db.Commit();
            db.Close();
            db.Dispose();

        }
    }
}

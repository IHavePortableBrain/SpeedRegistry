using SpeedRegistry.Data.Entites;
using SpeedRegistry.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SpeedRegistry.Core;

namespace SpeedRegistry.Data.FileSystem.Repositories
{
    public class SpeedEntryRepository : ISpeedEntryRepository
    {
        public const string JsonExtension = ".json"; // todo: have a constants class.
        public const string Directory = "speed";
        public static readonly long ClasterTicksSize = new TimeSpan(1, 0, 0, 0).Ticks;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public SpeedEntryRepository()
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        public async Task<SpeedEntry> CreateAsync(SpeedEntry entity) //since inserts are rare, and entities will be ordered by time
        {
            var timer = Stopwatch.StartNew();
            var fileName = GetStorageFileName(entity.DateTime);
            var filePath = Path.Combine(Directory, fileName);

            if (!File.Exists(filePath))
            {
                var emptyJsonArray = "[]"; // get odd comma
                using (var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                using (var sw = new StreamWriter(file))
                {
                    await sw.WriteAsync(emptyJsonArray);
                }
            }

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                var json = "," + JsonConvert.SerializeObject(entity) + "]";
                var insertionPostion = -Encoding.GetByteCount("]");
                file.Seek(insertionPostion, SeekOrigin.End);
                var jsonBytes = Encoding.GetBytes(json);
                await file.WriteAsync(jsonBytes, 0, json.Length);
            }
            
            timer.Stop();
            Console.WriteLine("Выполнение метода заняло {0} мс", timer.ElapsedMilliseconds);

            return entity;
        }

        public void CreateRangeAsync(IEnumerable<SpeedEntry> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SpeedEntry>> FilterAsync(Period period)
        {
            await Task.CompletedTask;
            return null;
            // var entries = JsonConvert.DeserializeObject<IEnumerable<SpeedEntry>>(filePath);
        }

        private string GetStorageFileName(DateTime dateTime)
        {
            var ticks = dateTime.Date.Ticks - dateTime.Date.Ticks % ClasterTicksSize;
            return ticks + JsonExtension;
        }
    }
}

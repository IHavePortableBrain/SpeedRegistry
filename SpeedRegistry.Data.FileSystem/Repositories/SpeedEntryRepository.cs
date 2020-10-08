using SpeedRegistry.Data.Entites;
using SpeedRegistry.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SpeedRegistry.Core;
using System.Linq;
using System.Collections.Concurrent;

namespace SpeedRegistry.Data.FileSystem.Repositories
{
    public class SpeedEntryRepository : ISpeedEntryRepository
    {
        public long LastMethodElapsedMilliseconds { get; private set; }
        
        public const string JsonExtension = ".json"; // todo: have a constants class.
        public const string EmptyJsonArray = "[]";
        public const string Directory = "speed";
        public static readonly long ClusterSize = new TimeSpan(1, 0, 0, 0).Ticks;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public SpeedEntryRepository()
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        public async Task<SpeedEntry> CreateAsync(SpeedEntry entity) //since inserts to old files are not done, and entities ordered by time
        {
            var timer = Stopwatch.StartNew();
            var fileName = GetClusterFileName(entity.DateTime.Ticks);
            var filePath = Path.Combine(Directory, fileName);

            if (!File.Exists(filePath))
            {
                using (var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                using (var sw = new StreamWriter(file))
                {
                    await sw.WriteAsync(EmptyJsonArray);
                }
            }

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                var json = (file.Length > Encoding.GetByteCount(EmptyJsonArray) ? "," : string.Empty) // is not correct for jsons with indentions | spaces chars, but file will have neither indention or spaces
                    + JsonConvert.SerializeObject(entity) + "]";
                var insertionPostion = -Encoding.GetByteCount("]");
                file.Seek(insertionPostion, SeekOrigin.End);
                var jsonBytes = Encoding.GetBytes(json);
                await file.WriteAsync(jsonBytes, 0, json.Length);
            }

            StopTimer(timer);
            return entity;
        }

        public void CreateRangeAsync(IEnumerable<SpeedEntry> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SpeedEntry>> FilterAsync(ClosedPeriod period, Func<SpeedEntry, bool> predicate)
        {
            await Task.CompletedTask;
            var timer = Stopwatch.StartNew();
            var fileNames = GetOverrlappedClusterFileNames(period);
            var filePaths = fileNames.Select(n => Path.Combine(Directory, n));
            var existingPathes = filePaths.Where(p => File.Exists(p));
            var entries = new ConcurrentBag<SpeedEntry>();
            var partitioner = Partitioner.Create(existingPathes);
            Parallel.ForEach(partitioner, p =>
            //foreach (var p in existingPathes)
            {
                using (var file = new FileStream(p, FileMode.Open, FileAccess.Read))
                {
                    var bytes = new byte[file.Length];
                    file.Read(bytes, 0, bytes.Length);
                    var json = Encoding.GetString(bytes);
                    var jsonEntries = JsonConvert.DeserializeObject<IEnumerable<SpeedEntry>>(json);
                    foreach (var entry in jsonEntries)
                    {
                        entries.Add(entry);
                    }
                }
            }
            );

            var result = entries.Where(predicate);
            StopTimer(timer);
            return result;
        }

        private void StopTimer(Stopwatch timer)
        {
            timer.Stop();
            var callerMethodName = new StackTrace().GetFrame(1).GetMethod().Name;
            LastMethodElapsedMilliseconds = timer.ElapsedMilliseconds;
            Console.WriteLine($"Method '{callerMethodName}' elapsed {timer.ElapsedMilliseconds} ms");
        }

        private string GetClusterFileName(long ticks)
        {
            ticks -= ticks % ClusterSize;
            return ticks + JsonExtension;
        }

        private IEnumerable<string> GetOverrlappedClusterFileNames(ClosedPeriod period)
        {
            var result = new List<string>();
            var nowTicks = period.From.Ticks - period.From.Ticks % ClusterSize;
            while (nowTicks < period.To.Ticks)
            {
                result.Add(nowTicks + JsonExtension);
                nowTicks += ClusterSize;
            }

            return result;
        }
    }
}

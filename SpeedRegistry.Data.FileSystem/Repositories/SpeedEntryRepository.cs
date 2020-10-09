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
using System.Runtime.CompilerServices;

namespace SpeedRegistry.Data.FileSystem.Repositories
{
    public class SpeedEntryRepository : ISpeedEntryRepository
    {
        public long LastMethodElapsedMilliseconds { get; private set; }
        
        public const string JsonExtension = ".json"; // todo: have a constants class.
        public const string EmptyJsonArray = "[]"; // todo: json has storage overhead that can be reduced with use of custom text parser & storage format.
        public const string Directory = "speed";
        public static readonly long ClusterSize = new TimeSpan(1, 0, 0, 0).Ticks;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public SpeedEntryRepository()
        {
            System.IO.Directory.CreateDirectory(Directory);
            // todo: scan repo directory to determin MinStoredEntryDateTime ( = min ticks in file names) and Max ( = max ticks + ClusterSize). Then truncate all read requests to that period. Maintain new fields concistency on each write operation.
        }

        public async Task<SpeedEntry> CreateAsync(SpeedEntry entity)
        {
            return (await CreateRangeAsync(new List<SpeedEntry> { entity })).FirstOrDefault();
        }

        public async Task<IEnumerable<SpeedEntry>> CreateRangeAsync(IEnumerable<SpeedEntry> entities)
        {
            var timer = Stopwatch.StartNew();
            var entitiesByFilePath = entities
                .GroupBy(e => Path.Combine(Directory, GetClusterFileName(e.DateTime.Ticks)), e => e)
                .ToDictionary(g => g.Key, g => g.ToList());

            var partitioner = Partitioner.Create(entitiesByFilePath);
            //Parallel.ForEach(partitioner, p =>
            foreach (var kvp in entitiesByFilePath)
            {
                InitEmptyArrayJson(kvp.Key);

                using (var file = new FileStream(kvp.Key, FileMode.Open, FileAccess.Write))
                {
                    var json = (file.Length > Encoding.GetByteCount(EmptyJsonArray) ? "," : string.Empty) // is not correct for jsons with indentions | spaces chars, but file will have neither indention or spaces
                        + JsonConvert.SerializeObject(kvp.Value).Trim('[', ']')
                        + "]";
                    var insertionPostion = -Encoding.GetByteCount("]");
                    file.Seek(insertionPostion, SeekOrigin.End);
                    var jsonBytes = Encoding.GetBytes(json);
                    await file.WriteAsync(jsonBytes, 0, json.Length);
                }
            }
            StopTimer(timer);
            return entities;
        }

        public void CreateSortedRangeAsync(IEnumerable<SpeedEntry> entities)
        {
            throw new NotImplementedException(); // possible optimization based on fact that sensors take speed entires with strict ascending order
        }

        public async Task<IEnumerable<SpeedEntry>> FilterAsync(ClosedPeriod period, Func<SpeedEntry, bool> predicate) // todo: being killed when period.from --> dateTime.min and period.to --> dateTime.max
        {
            await Task.CompletedTask;
            var timer = Stopwatch.StartNew();
            var fileNames = GetOverrlappedClusterFileNames(period);
            var filePaths = fileNames.Select(n => Path.Combine(Directory, n));
            // var existingPaths = filePaths.Where(p => File.Exists(p));
            var entries = new ConcurrentBag<SpeedEntry>();
            var partitioner = Partitioner.Create(filePaths);
            Parallel.ForEach(partitioner, p =>
            //foreach (var p in filePaths)
            {
                if (File.Exists(p))
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

        private void InitEmptyArrayJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                using (var file = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                {
                    var bytes = Encoding.GetBytes(EmptyJsonArray);
                    file.Write(bytes);
                }
            }
        }

        private void StopTimer(Stopwatch timer, [CallerMemberName] string callerName = "SpeedEntryRepository.SomeMethod")
        {
            timer.Stop();
            //var callerMethodName = new StackTrace().GetFrame(4).GetMethod().Name;// todo: is not proper. Frame 4 was proper in particular case. 
            LastMethodElapsedMilliseconds = timer.ElapsedMilliseconds;
            Console.WriteLine($"Method '{callerName}' elapsed {timer.ElapsedMilliseconds} ms");
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

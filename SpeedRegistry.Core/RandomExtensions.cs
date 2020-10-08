namespace System
{
    public static class RandomExtensions
    {
        public const string LatinUpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static long NextLong(this Random random, long min = long.MinValue, long max = long.MaxValue)
        {
            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long next = BitConverter.ToInt64(buf, 0);
            return min + next % (max - min);
        }

        
        public static long NextLatinUpperChar(this Random random)
        {
            int next = random.Next(0, LatinUpperChars.Length);
            return LatinUpperChars[next];
        }

        public static string NextVehicleNumber(this Random random)
        {
            return $"{random.Next(0, 10_000)} {random.NextLatinUpperChar()}{random.NextLatinUpperChar()}-{random.Next(0, 10)}";
        }
    }
}

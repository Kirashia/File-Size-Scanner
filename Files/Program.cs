using System;
using System.Collections.Generic;
using System.IO;

namespace Files
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Filepath to check: ");
            string p = Console.ReadLine();

            try
            {
                DirectoryInfo di = new DirectoryInfo(@p);
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred trying to load this directory.");
                return;
            }

            Console.WriteLine("Threshold in bytes, KB, MB, or GB?");
            string format = Console.ReadLine().ToUpper();
            long coeff = 1;
            switch (format)
            {
                case "BYTES":
                    coeff = 1;
                    break;

                case "KB":
                    coeff = 1000;
                    break;

                case "MB":
                    coeff = 1000000;
                    break;

                case "GB":
                    coeff = 1000000000;
                    break;
            }

            Console.WriteLine("Minimum size check in {0} (0 for none):", format);
            long threshold = long.Parse(Console.ReadLine()) * coeff;

            GetDirectorySize(@p, threshold);

            Console.WriteLine("Scan finished. Press ANY KEY to continue.");
            Console.ReadKey();
        }

        public static void GetDirectorySize(string p, long threshold = 0, HashSet<string> done = null)
        {
            DirectoryInfo di;
            DirectoryInfo[] diArr = new DirectoryInfo[0];
            FileInfo[] allFiles = new FileInfo[0];

            try
            {
                di = new DirectoryInfo(@p);
                diArr = di.GetDirectories();
                allFiles = di.GetFiles("*.*", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Console.WriteLine("---------------- NEW DIRECTORY: {0} ----------------", p);
            if (done == null)
                done = new HashSet<string>();

            long size = 0;

            foreach(DirectoryInfo d in diArr)
            {
                GetDirectorySize(d.FullName, threshold, done);
            }

            foreach(FileInfo f in allFiles)
            {
                //Console.WriteLine("Checking: " + f.FullName);
                if (f.Length > threshold && !done.Contains(f.FullName))
                    Console.Write("\n{0} has a filesize of {1} GB ({3} bytes) ({2})", f.Name, GetGB(f.Length), f.FullName, f.Length);
                
                size += f.Length;
                done.Add(f.FullName);
            }

            if (size > threshold)
            {
                Console.Write("\nTotal size for {0}: ", p);

                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(GetGB(size));
                Console.ForegroundColor = cc;

                Console.Write(" GB ({0} bytes)", size);
                //Console.WriteLine("\n\n");
            }

        }

        public static float GetGB(long bytes)
        {
            if (bytes < MathF.Pow(10, 6))
                return 0;

            float v = MathF.Pow(10, -9);
            return bytes * v;
        }
    }
}

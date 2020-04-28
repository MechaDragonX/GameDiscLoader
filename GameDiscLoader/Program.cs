using DiscUtils.Iso9660;
using System;
using System.IO;

namespace GameDiscLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            DriveInfo discDrive = null;
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.CDRom)
                    discDrive = drive;
            }

            if (discDrive == null)
            {
                Console.WriteLine("No disc drive found!");
                return;
            }
            if (!discDrive.IsReady)
            {
                Console.WriteLine("No disc found? Is a disc inserted?");
                return;
            }

            CDBuilder builder = new CDBuilder();
            builder.UseJoliet = true;
            builder.VolumeIdentifier = "test";
            builder.VolumeIdentifier = discDrive.VolumeLabel;
            AddFilesRecursively(ref builder, discDrive.RootDirectory.FullName);
            Console.WriteLine("Files added successfully!");

            string outDir = "";
#if RELEASE
            outDir = Path.Combine(Environment.CurrentDirectory, "out");
            if(!Directory.Exists())
                Directory.CreateDirectory(outDir);
            builder.Build(Path.Combine(outDir, $"{ builder.VolumeIdentifier }.iso"));
#else
            outDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "out");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            builder.Build(Path.Combine(outDir, $"{ builder.VolumeIdentifier }.iso"));
#endif
            Console.WriteLine("ISO file created!");
            Console.ReadKey();
        }

        private static void AddFilesRecursively(ref CDBuilder builder, string path)
        {
            try
            {
                foreach(string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    builder.AddFile(Path.GetFileName(file), File.ReadAllBytes(file));
                    //foreach (string file in Directory.GetFiles(dir))
                    //{
                    //    builder.AddFile(Path.GetFileName(file), File.ReadAllBytes(file));
                    //}
                    //AddFilesRecursively(ref builder, path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
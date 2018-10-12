using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelPyz {
    class Program
    {
        static public string path;
        static void Main(string[] args)
        {
            try
            {
                path = Directory.GetCurrentDirectory();
                Console.WriteLine(path);
                string[] filelist = Directory.GetFiles(path);
                Comic comic;
                //foreach (string file in filelist)
                Parallel.ForEach(filelist, (file) =>
                {
                    if (Path.GetExtension(file).Equals(".cbr") || Path.GetExtension(file).Equals(".cbz"))
                    {
                        comic = new Comic(file);
                        comic.ExtractArchive();
                    }
                });
                Console.WriteLine("Complete!");
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }
    }
}

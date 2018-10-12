using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using System.Collections.Generic;
using System.IO;
using System;

namespace ParallelPyz {

    class Comic {
        string filepath;
        string dir_root;
        public string file_base;
        public string temp_dir;
        string out_path = @"d:\webpcomics";
        public MemoryStream converted = new MemoryStream();

        public Comic(string _filepath) {
            filepath = _filepath;
            dir_root = Path.GetDirectoryName(filepath);
        }

        public void ExtractArchive() {
            var archive = ArchiveFactory.Open(filepath);
            temp_dir = GetTemporaryDirectory();
            foreach (var entry in archive.Entries) {
                if (!entry.IsDirectory) {
                    string ek = entry.Key.ToLower();

                    if (entry.Key.ToLower().StartsWith("zz")) {
                        continue;
                    }
                    
                    file_base = Path.GetFileNameWithoutExtension(entry.Key);
                    MemoryStream memStream = new MemoryStream();
                    entry.WriteTo(memStream);
                    //Page temp_page = new Page(memStream, entry.Key);
                    //orig_pages.Add(temp_page);       
                    ISupportedImageFormat format = new WebPFormat { Quality = 80 };
                    ImageFactory image = new ImageProcessor.ImageFactory();
                    try
                    {
                        image.Load(memStream.ToArray())
                                    .Format(format)
                                    .Save(converted);
                    }
                    catch {
                        continue;
                    }
                    string filename = Path.Combine(temp_dir, file_base + ".webp");
                    using (System.IO.FileStream output = new System.IO.FileStream(filename, FileMode.Create)) {
                        converted.CopyTo(output);
                        memStream.Dispose();
                        output.Close();
                    }
                }
            }
            CompressArchive();
        }

        public void CompressArchive() {
            string outFile = Path.GetFileNameWithoutExtension(filepath) + ".cbz";
            using (var archive = ZipArchive.Create()) {
                archive.AddAllFromDirectory(temp_dir);
                using (Stream newStream = File.Create(Path.Combine(out_path, outFile))) {
                    archive.SaveTo(newStream, SharpCompress.Common.CompressionType.LZMA);
                    newStream.Dispose();
                    System.Console.WriteLine(Path.Combine(out_path, outFile));
                }
            }         
            Directory.Delete(temp_dir, true);
        }

        static string GetTemporaryDirectory() {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace Utility.HelperClasses
{
    public class FileUtility
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static List<string> GetFilesPath(string path)
        {
            List<string> filespath = new List<string>();
            foreach (var item in Directory.GetFiles(path))
            {
                filespath.Add(item);
            }
            return filespath;
        }
        public static void DeleteFilesFromPath(string path)
        {
            CreateDirectory(path);
            System.IO.DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            Directory.Delete(path);
        }
        public static Tuple<bool, string> UploadFile(IFormFile file, string FolderName, string FolderPath)
        {
            try
            {
                string UploadFolder = Path.Combine(FolderPath, FolderName);

                if (!Directory.Exists(UploadFolder))
                    Directory.CreateDirectory(UploadFolder);

                string UniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName.Replace('_', '-'); ;
                string FilePath = Path.Combine(UploadFolder, UniqueFileName);
                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Tuple.Create(true, FilePath);
            }
            catch (Exception)
            {
                return Tuple.Create<bool, string>(false, null);
            }

        }
    }
}
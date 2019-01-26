using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CropImage.Commons
{
    public class FileHelper
    {
        public static void CreateFolderIfNotExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CopyFolderToFolder(string source, string target)
        {


            DirectoryInfo StartDirectory = new DirectoryInfo(source);
            DirectoryInfo EndDirectory = new DirectoryInfo(target);
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(source, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(source, target));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(source, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(source, target), true);
        }

        public static void Rename(string oldName, string newName)
        {
            System.IO.File.Move(oldName, newName);
        }

        public static string ReadAllText(string fileName)
        {
            string text = System.IO.File.ReadAllText(fileName);
            return text;
        }

        public static void ReplaceText(string fileName, string oldText, string newText)
        {
            string text = File.ReadAllText(fileName, Encoding.Unicode);
            text = text.Replace(oldText, newText);
            File.WriteAllText(fileName, text);
        }
        public static void CreateFile(string folder, string fileName, string text)
        {
            CreateFolderIfNotExist(folder);
            string filePath = "";
            if (folder.EndsWith(@"\")) filePath = folder + fileName;
            else filePath = folder + @"\" + fileName;
            CreateFile(filePath, text);
        }
        public static async Task CreateFileAsync(string folder, string fileName, string text)
        {
            CreateFolderIfNotExist(folder);
            string filePath = "";
            if (folder.EndsWith(@"\")) filePath = folder + fileName;
            else filePath = folder + @"\" + fileName;
            using (StreamWriter sw = (File.Exists(filePath)) ? File.CreateText(filePath) : File.CreateText(filePath))
            {
                await sw.WriteAsync(text);
            }
        }
        public static string GetRunningPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
        public static bool DeleteFolder(string folder)
        {
            if (System.IO.Directory.Exists(folder))
            {
                try
                {
                    System.IO.Directory.Delete(folder, true);
                    return true;
                }

                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return false;
        }
        public static void CreateFile(string filePath, string text)
        {
            using (StreamWriter sw = (File.Exists(filePath)) ? File.CreateText(filePath) : File.CreateText(filePath))
            {
                sw.Write(text);
            }
        }
        public static void CreateFileLn(string filePath, string text)
        {
            using (StreamWriter sw = (File.Exists(filePath)) ? File.CreateText(filePath) : File.CreateText(filePath))
            {
                sw.WriteLine(text);
            }
        }
        public static async void CreateFileLnAsync(string filePath, string text)
        {
            using (StreamWriter sw = (File.Exists(filePath)) ? File.CreateText(filePath) : File.CreateText(filePath))
            {
               await  sw.WriteLineAsync(text);
            }
        }

        public static void CoppyFile(string Source, string Taget)
        {
            System.IO.File.Copy(Source, Taget);
        }
        public static void AppenAllText(string path, string data)
        {
            File.AppendAllText(path, data);
        }
    }
}
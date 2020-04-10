using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sl.Bpm.CodeTool.Common
{
    public class FileUtility
    {
        private static FileUtility fUtility = new FileUtility();
        private Encoding encoding = new UTF8Encoding(false);

        public static string TempPath
        {
            get
            {
                return Path.GetTempPath();
            }
        }


        /// <summary>
        /// 根据相对路径,返回文件内容
        /// </summary>
        /// <param name="fileName">文件相对路径</param>
        /// <returns></returns>
        public static string FileContent(string fileName)
        {
            return fUtility.GetFileContent(fileName);
        }

        public static void WriteAllText(string path, string content)
        {
            fUtility.SaveFile(path, content, true);
        }


        public static void Save(string fileName,string content)
        {
            fUtility.SaveFile(fileName, content);
        }
        /// <summary>
        /// 根据相对路径,返回文件hash
        /// </summary>
        /// <param name="fileName">文件相对路径</param>
        /// <returns></returns>
        public static int FileHash(string fileName)
        {
            return fUtility.GetFileHash(fileName);
        }
       
        private int GetFileHash(string file)
        {
            var content = GetFileContent(file);
            if (content == null)
            {
                return 0;
            }
            return content.GetHashCode();
        }
        private string GetFileContent(string file,bool absPath=false)
        {
            var fileName = absPath ? file : GetFilePath(file);
            if (!File.Exists(fileName))
            {
                MessageBox.Show($"{fileName}工作版本丢失!");
                return null;
            }
            using (StreamReader sr = new StreamReader(fileName, encoding))
            {
                return sr.ReadToEnd();
            }
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(AppInfo.AppRootPath, "AppPages" + fileName); 
        }

        public void SaveFile(string fileName, string content, bool absPath = false)
        {
            var path = absPath ? fileName : GetFilePath(fileName);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (StreamWriter sw = new StreamWriter(path, false, encoding))
            {
                sw.Write(content);
            }
        }


    }
}

using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;

namespace GXPEngine
{
    /// <summary>
    /// Load TMX file names in Debug/Release Folder
    /// </summary>
    public class TmxFilesLoader
    {
        public static string[] GetTmxFileNames()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var files = new DirectoryInfo(baseDir)?.GetFiles("*.tmx");
            
            return files?.Select(f => f.FullName).ToArray();
        }
    }
}
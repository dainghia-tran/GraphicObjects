using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GraphicsLibrary
{
    public class DllLoader
    {
        public static List<Type> Types { get; set; }
        public static void execute()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string folder = Path.GetDirectoryName(exePath);
            FileInfo[] fileInfos = new DirectoryInfo(folder).GetFiles("*.dll");
            Types = new List<Type>();
            foreach (FileInfo fileInfo in fileInfos)
            {
                Assembly assembly = Assembly.LoadFrom(fileInfo.FullName);
                Type[] types = assembly.GetTypes();

                Types.AddRange(types);
            }
        }
    }
}

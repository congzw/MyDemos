using System;
using System.IO;
using System.Reflection;
using Demos.AsciiTrees;

namespace Demos
{
    class Program
    {
        static void Main(string[] args)
        {
            var asciiTreeHelper = new AsciiTreeHelper();
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var exeFileInfo = new FileInfo(new Uri(codeBase).AbsolutePath);
            asciiTreeHelper.EscapePaths.Add(exeFileInfo.FullName);
            var appendRoot = !Assembly.GetExecutingAssembly().CodeBase.ToLower().Contains("_noroot");
            var asciiTreeText = asciiTreeHelper.GetCurrentDirectoryAsciiTreeText(appendRoot);
            Console.WriteLine(asciiTreeText);
            File.WriteAllText("AsciiTreeText.txt", asciiTreeText);
            Console.Read();
        }
    }
}

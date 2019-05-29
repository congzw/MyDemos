using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Demos.AsciiTrees
{
    public class AsciiTreeHelper
    {
        public const string _cross = " ├─";
        public const string _corner = " └─";
        public const string _vertical = " │ ";
        public const string _space = "   ";

        public AsciiTreeHelper()
        {
            EscapePaths = new List<string>();
        }

        public IList<string> EscapePaths { get; set; }

        public int MaxPrintDeep { get; set; }

        public void ProcessNode(AsciiTree node, string indent, StringBuilder sb, int currentDeep)
        {
            sb.AppendLine(node.Name);
            var numberOfChildren = node.Children.Count;
            for (var i = 0; i < numberOfChildren; i++)
            {
                var child = node.Children[i];
                var isLast = (i == (numberOfChildren - 1));
                ProcessChildNode(child, indent, isLast, sb, currentDeep + 1);
            }
        }

        private void ProcessChildNode(AsciiTree node, string indent, bool isLast, StringBuilder sb, int currentDeep)
        {
            if (MaxPrintDeep != 0 && currentDeep > MaxPrintDeep)
            {
                return;
            }
            // Print the provided pipes/spaces indent
            sb.Append(indent);

            // Depending if this node is a last child, print the
            // corner or cross, and calculate the indent that will
            // be passed to its children
            if (isLast)
            {
                sb.Append(_corner);
                indent += _space;
            }
            else
            {
                sb.Append(_cross);
                indent += _vertical;
            }

            ProcessNode(node, indent, sb, currentDeep);
        }

        public string GetCurrentDirectoryAsciiTreeText(bool appendRoot)
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var directoryInfo = new DirectoryInfo(currentDirectory);
            return GetDirectoryAsciiTreeText(directoryInfo, appendRoot);
        }

        public string GetDirectoryAsciiTreeText(DirectoryInfo dirInfo, bool appendSelfDir)
        {
            if (dirInfo == null)
            {
                return null;
            }

            var node = new AsciiTree();
            node.Name = dirInfo.Name;
            AppendDirectory(node, dirInfo);

            var sb = new StringBuilder();
            if (appendSelfDir)
            {
                ProcessNode(node, "", sb, 0);
            }
            else
            {
                foreach (var child in node.Children)
                {
                    ProcessNode(child, "", sb, 0);
                }
            }
            return sb.ToString().TrimEnd();
        }

        private void AppendDirectory(AsciiTree node, DirectoryInfo directoryInfo)
        {
            if (ShouldEscape(directoryInfo))
            {
                return;
            }

            node.Name = directoryInfo.Name;
            AppendFiles(node, directoryInfo.GetFiles());
            
            //child dir
            foreach (var childDir in directoryInfo.GetDirectories())
            {
                var childDirNode = new AsciiTree();
                node.Children.Add(childDirNode);
                AppendDirectory(childDirNode, childDir);
            }
        }
        private void AppendFiles(AsciiTree node, FileInfo[] fileInfos)
        {
            if (fileInfos == null || fileInfos.Length == 0)
            {
                return;
            }

            //child files
            foreach (var fileInfo in fileInfos)
            {
                if (ShouldEscape(fileInfo))
                {
                    continue;
                }
                var childFileNode = new AsciiTree();
                childFileNode.Name = fileInfo.Name;
                node.Children.Add(childFileNode);
            }
        }

        private bool ShouldEscape(FileSystemInfo info)
        {
            if (info == null || EscapePaths == null || EscapePaths.Count == 0)
            {
                return false;
            }

            foreach (var escapePath in EscapePaths)
            {
                if (escapePath == null)
                {
                    return true;
                }
                var fileInfo = new FileInfo(escapePath);
                if (info.FullName.Equals(fileInfo.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
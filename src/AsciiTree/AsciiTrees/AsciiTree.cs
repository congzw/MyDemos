using System.Collections.Generic;

namespace Demos.AsciiTrees
{
    public class AsciiTree
    {
        public string Name { get; set; }
        public List<AsciiTree> Children { get; } = new List<AsciiTree>();
    }
}
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Demos.SplitScreens
{
    public class SplitScreenConfig
    {
        public SplitScreenConfig()
        {
            Uris = new List<PlayUri>();
            Margin = new Thickness(0);
            PlayerBorderColor = Colors.Red.ToString();
        }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int Distance { get; set; }
        public IList<PlayUri> Uris { get; set; }

        public bool ShowPlayerBorder { get; set; }
        public string PlayerBorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public Thickness? Margin { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Demos.Common;
using Demos.SplitScreens;
using Demos.ViewModel;

namespace Demos
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowJs MainWindowJs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            MainWindowJs = new MainWindowJs(this);
            this.BtnPrepare.Click += BtnPrepare_Click;
            this.BtnHide.Click += BtnHide_Click;
            this.BtnChangeSize.Click += BtnChangeSize_Click;
            this.BtnChangePlayers.Click += BtnChangePlayers_Click;
            this.BtnInitPlayers.Click += BtnInitPlayers_Click;
        }

        private void BtnPrepare_Click(object sender, RoutedEventArgs e)
        {
            BtnChangeSize.Content = "Normal Size";
            Message.Content = "";
            var columnCount = FindSelectCount("ColumnCount", 3);
            var rowCount = FindSelectCount("RowCount", 2);
            var controlCount = FindSelectCount("ControlCount", 5);
            Message.Content = string.Format($"{rowCount}x{columnCount}: {controlCount}");

            var config = new SplitScreenConfig();
            config.ColumnCount = columnCount;
            config.RowCount = rowCount;
            config.Distance = 10;
            var wrapperCount = rowCount * columnCount;
            
            //mock generate test config
            var randomUris = CreateRandomUris(wrapperCount, controlCount);
            config.Uris = randomUris;

            config.ShowPlayerBorder = true;
            config.PlayerBorderColor = "Blue";
            config.BackgroundColor = "Green";

            //var players = new Dictionary<int, Player>();
            var helper = new SplitScreenHelper();

            //set by config
            this.SplitScreen.Background = new SolidColorBrush(Colors.Gray);

            //init
            helper.Init(this.SplitScreen, config);
        }

        private void BtnHide_Click(object sender, RoutedEventArgs e)
        {
            var visible = this.SplitScreen.Visibility == Visibility.Visible;
            if (visible)
            {
                this.BtnHide.Content = "Show";
                MainWindowJs.HidePlayers();
            }
            else
            {
                this.BtnHide.Content = "Hide";
                MainWindowJs.ShowPlayers();
            }
        }
        
        private bool playing = false;
        private void BtnChangePlayers_Click(object sender, RoutedEventArgs e)
        {
            if (!playing)
            {
                MainWindowJs.ChangePlayers(new
                {
                    Uris = new[]
                    {
                        new {Id = 0, Uri = "http://localhost/uri_0"},
                        new {Id = 1, Uri = "http://localhost/uri_1"},
                        new {Id = 2, Uri = "http://localhost/uri_2"},
                        new {Id = 4, Uri = "http://localhost/uri_4"}
                    }
                });
            }
            else
            {
                MainWindowJs.ChangePlayers(new
                {
                    Uris = new[]
                    {
                        new {Id = 0, Uri = ""},
                        new {Id = 1, Uri = ""},
                        new {Id = 2, Uri = ""},
                        new {Id = 4, Uri = "http://localhost/uri_4"}
                    }
                });
            }

            playing = !playing;
        }

        private void BtnChangeSize_Click(object sender, RoutedEventArgs e)
        {
            if (BtnChangeSize.Content.ToString() != "Normal Size")
            {
                this.BtnChangeSize.Content = "Normal Size";
                MainWindowJs.ChangePlayers(new { Margin = new { Left = 0, Top = 0, Right = 0, Bottom = 0 } });
            }
            else
            {
                this.BtnChangeSize.Content = "Full Size";
                MainWindowJs.ChangePlayers(new { Margin = new { Left = 200, Top = 0, Right = 0, Bottom = 200 } });
            }
        }

        private void BtnInitPlayers_Click(object sender, RoutedEventArgs e)
        {
            var initConfig = GetOneConfig();
            MainWindowJs.InitPlayers(initConfig);
        }
        
        private int FindSelectCount(string name, int defaultValue)
        {
            foreach (var child in this.MyStackPanel.Children)
            {
                if (child is RadioButton)
                {
                    var theRbtn = (RadioButton)child;
                    var groupName = theRbtn.GroupName;
                    if (groupName == name && theRbtn.IsChecked.HasValue && theRbtn.IsChecked.Value)
                    {
                        return int.Parse(theRbtn.Content.ToString());
                    }
                }
            }
            return defaultValue;
        }

        private IList<PlayUri> CreateRandomUris(int totalCount, int controlCount)
        {
            //mock generate test config
            var randomUris = new Dictionary<int, PlayUri>();
            if (controlCount >= totalCount)
            {
                controlCount = totalCount;
            }
            
            while (randomUris.Count < controlCount)
            {
                var nextId = RandomHelper.GetRandom(0, totalCount);
                if (!randomUris.ContainsKey(nextId))
                {
                    randomUris.Add(nextId, new PlayUri() { Id = nextId, Uri = "" });
                }
            }
            return randomUris.Select(x => x.Value).OrderBy(x => x.Id).ToList();
        }

        private IList<SplitScreenConfig> GetDemoInitConfigs()
        {
            var configs = new List<SplitScreenConfig>();
            configs.Add(new SplitScreenConfig() { BackgroundColor = Colors.Gray.ToString(), RowCount = 1, ColumnCount = 1 });
            configs.Add(new SplitScreenConfig() { BackgroundColor = Colors.Aqua.ToString(), RowCount = 1, ColumnCount = 2 });
            configs.Add(new SplitScreenConfig() { BackgroundColor = Colors.Yellow.ToString(), RowCount = 2, ColumnCount = 2, Margin = new Thickness(100, 0, 0, 100) });
            configs.Add(new SplitScreenConfig() { BackgroundColor = "Bad", RowCount = 3, ColumnCount = 2 });
            configs.Add(new SplitScreenConfig() { BackgroundColor = Colors.Green.ToString(), RowCount = 3, ColumnCount = 3, Margin = new Thickness(100, 0, 0, 100)});
            foreach (var config in configs)
            {
                config.ShowPlayerBorder = true;
                config.Distance = 10;
                var totalCount = config.RowCount * config.ColumnCount;
                config.Uris = CreateRandomUris(totalCount, totalCount);
            }
            return configs;
        }

        private int initIndex = 0;
        private SplitScreenConfig GetOneConfig()
        {
            var initConfigs = GetDemoInitConfigs();
            var currentIndex = initIndex % initConfigs.Count;
            initIndex++;
            var initConfig = initConfigs[currentIndex];
            return initConfig;
        }
    }
}

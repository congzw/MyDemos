using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Demos.Common;
using Demos.Controls;
using Demos.SplitScreens;

namespace Demos.ViewModel
{
    public class MainWindowJs
    {
        public MainWindow TheWindow { get; set; }
        public MainWindowJs(MainWindow mainWindow)
        {
            TheWindow = mainWindow;
        }
        
        public void InitPlayers(dynamic config)
        {
            var dynamicHelper = DynamicHelper.Instance;
            var theConfig = dynamicHelper.Convert<SplitScreenConfig>(config, false);
            if (theConfig == null)
            {
                return;
            }

            var helper = new SplitScreenHelper();
            var players = helper.GetPlayers(TheWindow.SplitScreen);
            foreach (var player in players)
            {
                var playerValue = player.Value;
                playerValue?.Stop();
            }
            
            //init
            helper.Init(TheWindow.SplitScreen, config);
        }

        public void ShowPlayers()
        {
            TheWindow.Dispatcher.Invoke(() => TheWindow.SplitScreen.Visibility = Visibility.Visible);
        }

        public void HidePlayers()
        {
            TheWindow.Dispatcher.Invoke(() => TheWindow.SplitScreen.Visibility = Visibility.Hidden);
        }

        public void ChangePlayers(dynamic config)
        {
            //todo: log config
            TheWindow.Dispatcher.Invoke(() =>
            {
                var gridHelper = new SplitScreenHelper();
                var players = gridHelper.GetPlayers(TheWindow.SplitScreen);
                //var players = TryFindPlayers(TheWindow.MyUniformGrid);
                if (players == null || players.Count == 0)
                {
                    return;
                }

                var dynamicHelper = DynamicHelper.Instance;
                if (dynamicHelper.IsPropertyExist(config, "Margin"))
                {
                    if (config.Margin != null)
                    {
                        var margin = config.Margin;
                        TheWindow.SplitScreen.Margin = new Thickness(margin.Left, margin.Top, margin.Right, margin.Bottom);
                    }
                }

                //兼容以前
                if (dynamicHelper.IsPropertyExist(config, "Uri"))
                {
                    string theUri = config.Uri;
                    var thePlayer = players.Values.FirstOrDefault();
                    PlayUri(thePlayer, theUri);
                }

                //批量操作
                if (dynamicHelper.IsPropertyExist(config, "Uris"))
                {
                    foreach (dynamic item in config.Uris)
                    {
                        if (item != null)
                        {
                            if (dynamicHelper.IsPropertyExist(item, "Id") && dynamicHelper.IsPropertyExist(item, "Uri"))
                            {
                                int theIndex = item.Id;
                                string theUri = item.Uri;

                                if (players.ContainsKey(theIndex))
                                {
                                    var thePlayer = players[theIndex];
                                    PlayUri(thePlayer, theUri);
                                }
                            }
                        }
                    }
                }
            });
        }

        private IList<Player> TryFindPlayers(UniformGrid uniformGrid)
        {
            var gridHelper = new SplitScreenHelper();
            var players = gridHelper.GetPlayers(uniformGrid);
            return players.Select(x => x.Value).ToList();
        }

        private void PlayUri(Player player, string uri)
        {
            if (player != null)
            {
                player.Stop();
                player.Content = "stopped";

                if (!string.IsNullOrWhiteSpace(uri))
                {
                    player.Play(uri);
                    player.Content = "playing: " + uri;
                }
            }
        }
    }
}

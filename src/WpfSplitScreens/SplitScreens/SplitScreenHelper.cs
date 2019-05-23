using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Demos.Controls;

namespace Demos.SplitScreens
{
    public class SplitScreenHelper
    {
        public void Init(UniformGrid uniformGrid, SplitScreenConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            var backgroundColor = TryConvertColor(config.BackgroundColor, Colors.Gray);
            uniformGrid.Background = new SolidColorBrush(backgroundColor);
            FillWrappers(uniformGrid, config.RowCount, config.ColumnCount, config.Distance, config.Margin);

            var players = GetPlayers(uniformGrid);
            foreach (var player in players)
            {
                var playerValue = player.Value;
                playerValue?.Stop();
            }

            var wrapperCount = config.RowCount * config.ColumnCount;
            var initPlayers = CreatePlayers(wrapperCount, config.Uris);
            FillPlayers(uniformGrid, initPlayers, config.ShowPlayerBorder, config.PlayerBorderColor);
        }

        public IDictionary<int, Player> GetPlayers(UniformGrid uniformGrid)
        {
            var players = new Dictionary<int, Player>();
            if (uniformGrid == null)
            {
                return players;
            }
            var index = 0;
            foreach (var panel in uniformGrid.Children)
            {
                if (panel is Border wrapper)
                {
                    if (wrapper.Child is Player player)
                    {
                        players.Add(index, player);
                    }
                }
                index++;
            }
            return players;
        }

        public IDictionary<int, Player> CreatePlayers(int wrapperCount, IEnumerable<PlayUri> uris)
        {
            var players = new Dictionary<int, Player>();
            var rnd = new Random();
            foreach (var uri in uris)
            {
                if (wrapperCount > uri.Id)
                {
                    var control = new Player() { Content = $"{uri.Id + 1}/{wrapperCount} " + typeof(Player).Name };
                    control.Background = PickRandomBrush(rnd);
                    players.Add(uri.Id, control);
                }
            }
            return players;
        }

        private void FillWrappers(UniformGrid uniformGrid, int rowCount, int columnCount, int distance, Thickness? margin)
        {
            var wrappers = uniformGrid.Children;
            wrappers.Clear();
            uniformGrid.Rows = rowCount;
            uniformGrid.Columns = columnCount;
            uniformGrid.Margin = margin ?? new Thickness(0, 0, 0, 0);

            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                for (int colIndex = 0; colIndex < columnCount; colIndex++)
                {
                    var theWrapper = new Border();
                    var rightThick = colIndex < columnCount - 1 ? 0 : distance;
                    var bottomThick = rowIndex < rowCount - 1 ? 0 : distance;
                    theWrapper.BorderThickness = new Thickness(distance, distance, rightThick, bottomThick);
                    theWrapper.BorderBrush = Brushes.Transparent;
                    wrappers.Add(theWrapper);
                }
            }
        }
        private void FillPlayers(UniformGrid uniformGrid, IDictionary<int, Player> players, bool showPlayerBorder, string playerBorderColor)
        {
            if (players == null || players.Count == 0)
            {
                return;
            }
            var wrappers = uniformGrid.Children;
            for (int i = 0; i < wrappers.Count; i++)
            {
                if (players.ContainsKey(i))
                {
                    var wrapper = (Border)wrappers[i];
                    wrapper.Child = players[i];

                    //如果是调试状态，增加一个边框，容易辨认
                    if (showPlayerBorder)
                    {
                        players[i].BorderBrush = new SolidColorBrush(TryConvertColor(playerBorderColor, Colors.Red));
                        players[i].BorderThickness = new Thickness(1, 1, 1, 1);
                    }
                }
            }
        }
        private Brush PickRandomBrush(Random rnd)
        {
            var brushesType = typeof(Brushes);
            var properties = brushesType.GetProperties();
            int random = rnd.Next(properties.Length);
            var result = (Brush)properties[random].GetValue(null, null);
            return result;
        }
        private Color TryConvertColor(string color, Color defaultColor)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(color))
                {
                    var converter = new ColorConverter();
                    var theColor = (Color)converter.ConvertFromInvariantString(color);
                    return theColor;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return defaultColor;
        }
    }
}
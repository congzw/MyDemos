using System.Windows.Controls;

namespace Demos.Controls
{
    public class Player : UserControl
    {
        public bool IsPlaying { get; private set; }

        public string GetCurrentStreamUrl()
        {
            return string.Empty;
        }

        public void Play(object streamUrl)
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }
    }
}

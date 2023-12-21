using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_2048
{
    public class GameTile : Grid
    {
        public Rectangle Body = new Rectangle() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, RadiusX = 10, RadiusY = 10};
        public TextBlock TBlock = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        public GameTile()
        {
            Children.Add(Body);
            Children.Add(TBlock);
            SizeChanged += GameTile_SizeChanged;
            
        }

        private void GameTile_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Body.Height = Height;
            Body.Width = Width;
        }
    }
}

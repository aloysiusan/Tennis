using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tennis.ApplicationGUI
{
    class BitmapConverter
    {
        static private readonly double DPI = 180;
        public static readonly double SCALE = DPI / 96;

        public static WriteableBitmap CreateWriteableBitmapFromCanvas(Canvas pCanvas)
        {         
            Size size = new Size(pCanvas.ActualWidth, pCanvas.ActualHeight);

            pCanvas.Measure(size);
            pCanvas.Arrange(new Rect(size));

            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
              (int)(size.Width * SCALE),
              (int)(size.Height * SCALE),
              DPI,
              DPI,
              PixelFormats.Default);
            renderBitmap.Render(pCanvas);
            return new WriteableBitmap(renderBitmap);
        }
    }
}

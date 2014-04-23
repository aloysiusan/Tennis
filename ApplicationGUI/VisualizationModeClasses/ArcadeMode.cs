using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tennis.Design;
using Tennis.Shapes;
using Tennis.TEventArgs;

namespace Tennis.ApplicationGUI
{
    class ArcadeMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;
        private WriteableBitmap mainBitmap;
        private static ArcadeMode _currentInstance;
        private Stopwatch Watcher;

        public event EventHandler<TennisEventArgs> finishDrawingDesign_EventHandler;

        public static ArcadeMode createInstance(TDesign pDesign, Designer pDesigner)
        {
            _currentInstance = new ArcadeMode(pDesign, pDesigner);
            return _currentInstance;
        }

        public static ArcadeMode Instance()
        {
            return _currentInstance;
        }

        private ArcadeMode(TDesign pDesign, Designer pDesigner)
        {
            currentDesign = pDesign;
            mainDesigner = pDesigner;
            Watcher = new Stopwatch();
        }

        public override void initDrawing()
        {           
            Watcher.Start();
            mainDesigner.root.Children.Clear();
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
            this.drawCustomLines(currentDesign.customLines);
            this.drawCustomEllipses(currentDesign.customEllipses);
            mainBitmap = BitmapConverter.CreateWriteableBitmapFromCanvas(mainDesigner.root);

            foreach (TPoint fillPoint in currentDesign.fillIndicators)
            {
                this.paint(fillPoint);
            }

            Watcher.Stop();

            EventHandler<TennisEventArgs> handler = finishDrawingDesign_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.DrawDuration = (float)Watcher.Elapsed.TotalMilliseconds;
                args.VisualizationMode = TennisEventArgs.Mode.ARCADE;
                args.DesignBitmap = mainBitmap;
                handler(this, args);
            }

        }

        private void drawBorderLines(TLine[] pLines)
        {
            foreach (TLine line in pLines)
            {
                drawLine(line);
            }
        }

        private void drawLine(TLine pLine)
        {
            Line line = new Line();
            line.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pLine.color));
            line.X1 = pLine.startPoint.XPosition + TLine.POSITION_OFFSET;
            line.X2 = pLine.endPoint.XPosition + TLine.POSITION_OFFSET;
            line.Y1 = pLine.startPoint.YPosition + TLine.POSITION_OFFSET;
            line.Y2 = pLine.endPoint.YPosition + TLine.POSITION_OFFSET;
            line.StrokeThickness = pLine.thickness;

            mainDesigner.AddShape(line);
        }

        private void drawBorderArcs(TArc[] pArcs)
        {
            foreach (TArc arc in pArcs)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(arc.startPoint.XPosition + TArc.POSITION_OFFSET, arc.startPoint.YPosition + TArc.POSITION_OFFSET);
                SweepDirection direction = !(arc.inverted) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                figure.Segments.Add(new ArcSegment(new Point(arc.endPoint.XPosition + TArc.POSITION_OFFSET, arc.endPoint.YPosition + TArc.POSITION_OFFSET), new Size(100, 100), 0, false, direction, true));
                pathGeometry.Figures.Add(figure);
                Path path = new Path();
                path.Data = pathGeometry;
                path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(arc.color));
                path.StrokeThickness = arc.thickness;
                mainDesigner.AddShape(path);
            }
        }

        private void drawCustomLines(List<TLine> pLines)
        {            
            foreach (TLine line in pLines)
            {
                drawLine(line);
            }
        }

        private void drawEllipse(TEllipse pEllipse)
        {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Height = pEllipse.radius*2;
            newEllipse.Width = pEllipse.radius*2;
            newEllipse.StrokeThickness = pEllipse.thickness;
            newEllipse.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.fillColor));
            newEllipse.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.borderColor));
            Canvas.SetLeft(newEllipse, pEllipse.radiusPoint.XPosition - pEllipse.radius / 2);
            Canvas.SetTop(newEllipse, pEllipse.radiusPoint.YPosition - pEllipse.radius / 2);
            mainDesigner.AddShape(newEllipse);
        }


        private void drawCustomEllipses(List<TEllipse> pEllipses)
        {
            foreach (TEllipse ellipse in pEllipses)
            {
                drawEllipse(ellipse);
            }
        }

        private void paint(TPoint fillPoint)
        {
            fillPoint.adjustPosition(mainDesigner.ActualWidth, mainDesigner.ActualHeight);

            Point pt = new Point(fillPoint.XPosition * BitmapConverter.SCALE, fillPoint.YPosition * BitmapConverter.SCALE); //Debe multiplicarse por una constante K tal que K = DPI/96 para que pinte el area correcta
            //debido a la escala utilizada para crear el bitmap.
            Color newColor = (Color)ColorConverter.ConvertFromString(fillPoint.fillColor);
            Color oldColor = (Color)ColorConverter.ConvertFromString(fillPoint.oldColor);
            try
            {
                Stack<Point> StackPoint = new Stack<Point>();
                StackPoint.Push(pt);
                while (StackPoint.Count != 0)
                {
                    pt = StackPoint.Pop();
                    Color CurrentColor = GetPixelColor((int)pt.X, (int)pt.Y);
                    if (ColorMatch(CurrentColor, oldColor))
                    {
                        SetPixelColor((int)pt.X, (int)pt.Y, newColor);
                        StackPoint.Push(new Point(pt.X - 1, pt.Y));
                        StackPoint.Push(new Point(pt.X + 1, pt.Y));
                        StackPoint.Push(new Point(pt.X, pt.Y - 1));
                        StackPoint.Push(new Point(pt.X, pt.Y + 1));
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private bool ColorMatch(Color a, Color b)
        {
            int diff = 10;
            return (a.A - diff < b.A && a.A + diff > b.A && a.R - diff < b.R && a.R + diff > b.R &&
                        a.G - diff < b.G && a.G + diff > b.G && a.B - diff < b.B && a.B + diff > b.B);
        }

        private Color GetPixelColor(int x, int y)
        {
            var pixels = new byte[4];
            mainBitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void SetPixelColor(int x, int y, Color newColor)
        {
            var pixels = new byte[] { newColor.B, newColor.G, newColor.R, newColor.A }; // Blue Green Red Alpha
            mainBitmap.WritePixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
        }
    }
}

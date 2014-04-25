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
using System.Windows.Threading;
using Tennis.Design;
using Tennis.Shapes;
using Tennis.TEventArgs;

namespace Tennis.ApplicationGUI.VisualizationModes
{
    class ArcadeMode : VisualizationMode
    {
        private TDesign _CurrentDesign;
        private Designer _MainDesigner;
        private WriteableBitmap _MainBitmap;
        private static ArcadeMode CurrentInstance;
        private Stopwatch _Watcher;

        public event EventHandler<TennisEventArgs> finishDrawingDesign_EventHandler;

        public static ArcadeMode createNewInstance(TDesign pDesign, Designer pDesigner)
        {
            CurrentInstance = new ArcadeMode(pDesign, pDesigner);
            return CurrentInstance;
        }

        public static ArcadeMode getInstance()
        {
            return CurrentInstance;
        }

        private ArcadeMode(TDesign pDesign, Designer pDesigner)
        {
            _CurrentDesign = pDesign;
            _MainDesigner = pDesigner;
            _Watcher = new Stopwatch();
        }

        public override void initDrawing()
        {           
            _Watcher.Start();

            this.drawBorderLines(_CurrentDesign.BorderLines);
            this.drawBorderArcs(_CurrentDesign.BorderArcs);
            this.drawLine(_CurrentDesign.BaseLine);
            this.drawCustomLines(_CurrentDesign.CustomLines);
            this.drawCustomEllipses(_CurrentDesign.CustomEllipses);
            _MainBitmap = BitmapConverter.CreateWriteableBitmapFromCanvas(_MainDesigner.root);

            foreach (TPoint fillPoint in _CurrentDesign.FillIndicators)
            {
                this.fillArea(fillPoint);
            }

            _Watcher.Stop();

            EventHandler<TennisEventArgs> handler = finishDrawingDesign_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.DrawDuration = (float)_Watcher.Elapsed.TotalMilliseconds;
                args.VisualizationMode = TennisEventArgs.Mode.ARCADE;
                args.DesignBitmap = _MainBitmap;
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

            _MainDesigner.AddShape(line);
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
                _MainDesigner.AddShape(path);
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
            Canvas.SetLeft(newEllipse, pEllipse.radiusPoint.XPosition - pEllipse.radius);
            Canvas.SetTop(newEllipse, pEllipse.radiusPoint.YPosition - pEllipse.radius);
            _MainDesigner.AddShape(newEllipse);
        }


        private void drawCustomEllipses(List<TEllipse> pEllipses)
        {
            foreach (TEllipse ellipse in pEllipses)
            {
                drawEllipse(ellipse);
            }
        }

        private void fillArea(TPoint fillPoint)
        {
            fillPoint.adjustPosition(_MainDesigner.ActualWidth, _MainDesigner.ActualHeight); //4 Tiempos

            Point pt = new Point(fillPoint.XPosition * BitmapConverter.SCALE, fillPoint.YPosition * BitmapConverter.SCALE); // 7 Tiempos
            Color newColor = (Color)ColorConverter.ConvertFromString(fillPoint.fillColor); // 4 Tiempos
            Color oldColor = (Color)ColorConverter.ConvertFromString(fillPoint.oldColor); // 4 Tiempos

            Stack<Point> StackPoint = new Stack<Point>(); // 3 Tiempos
            StackPoint.Push(pt); // 3 Tiempos
            while (StackPoint.Count != 0) // N
            {
                pt = StackPoint.Pop(); // 3 Tiempos
                Color CurrentColor = getPixelColor((int)pt.X, (int)pt.Y); // 5 Tiempos
                if (isColorMatch(CurrentColor, oldColor)) // 5 Tiempos
                {   
                    setPixelColor((int)pt.X, (int)pt.Y, newColor); // 4 Tiempos
                    StackPoint.Push(new Point(pt.X - 1, pt.Y)); // 7 Tiempos
                    StackPoint.Push(new Point(pt.X + 1, pt.Y)); // 7 Tiempos
                    StackPoint.Push(new Point(pt.X, pt.Y - 1)); // 7 Tiempos
                    StackPoint.Push(new Point(pt.X, pt.Y + 1)); // 7 Tiempos
                }
            }
        }

        /* Calculo O(n) y f(n)
         * 
         * 4 + 7 + 4 + 4 + 3 + 3 + N(3 + 5 + 5 + 4 + 7 + 7 + 7 + 7)
         * = 25 + N(45)
         * = 25 + 45N
         * 
         * f(n) = 25 + 45n
         * 
         * Duracion: O(n)
         */

        private bool isColorMatch(Color a, Color b)
        {
            int diff = 10;
            return (a.A - diff < b.A && a.A + diff > b.A && a.R - diff < b.R && a.R + diff > b.R &&
                        a.G - diff < b.G && a.G + diff > b.G && a.B - diff < b.B && a.B + diff > b.B);
        }

        private Color getPixelColor(int x, int y)
        {
            var pixels = new byte[4];
            _MainBitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void setPixelColor(int x, int y, Color newColor)
        {
            var pixels = new byte[] { newColor.B, newColor.G, newColor.R, newColor.A }; // Blue Green Red Alpha
            _MainBitmap.WritePixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
        }
    }
}

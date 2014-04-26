using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// <summary>
    /// Fire Visualization Mode: Draws designs in a faster way than Arcade Mode.
    /// </summary>
    public class FireMode : VisualizationMode
    {
        private FireMode(TDesign pDesign, Designer pDesigner)
        {
            _CurrentDesign = pDesign;
            _MainDesigner = pDesigner;
            _Watcher = new Stopwatch();
        }

        public static FireMode createInstance(TDesign pDesign, Designer pDesigner)
        {
            CurrentInstance = new FireMode(pDesign, pDesigner);
            return CurrentInstance;
        }

        public static FireMode Instance()
        {
            return CurrentInstance;
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

            foreach (TFillIndicator fillIndicator in _CurrentDesign.FillIndicators)
            {
                this.paint(fillIndicator);        
            }

            _Watcher.Stop();

            EventHandler<TennisEventArgs> handler = finishDrawingDesign_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.DrawDuration = (float)_Watcher.Elapsed.TotalMilliseconds;
                args.VisualizationMode = TennisEventArgs.Mode.FIRE;
                args.DesignBitmap = _MainBitmap;
                handler(this, args);
            }

        }

        private void drawBorderLines(TLine[] pLines) {
            foreach (TLine tline in pLines)
            {
                drawLine(tline);
            }
        }

        private void drawLine(TLine pLine)
        {
            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(pLine.StartPoint.XPosition + TLine.POSITION_OFFSET, pLine.StartPoint.YPosition + TLine.POSITION_OFFSET);
            line.EndPoint = new Point(pLine.EndPoint.XPosition + TLine.POSITION_OFFSET, pLine.EndPoint.YPosition + TLine.POSITION_OFFSET);

            Path path = new Path();
            path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pLine.Color)); ;
            path.StrokeThickness = pLine.Thickness;

            path.Data = line;
            _MainDesigner.AddShape(path);
        }

        private void drawBorderArcs(TArc[] pArcs)
        {
            foreach (TArc arc in pArcs)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(arc._StartPoint.XPosition + TArc.POSITION_OFFSET, arc._StartPoint.YPosition + TArc.POSITION_OFFSET);
                SweepDirection direction = !(arc._Inverted) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                figure.Segments.Add(new ArcSegment(new Point(arc._EndPoint.XPosition + TArc.POSITION_OFFSET, arc._EndPoint.YPosition + TArc.POSITION_OFFSET), new Size(100, 100), 0, false, direction, true));
                pathGeometry.Figures.Add(figure);
                Path path = new Path();
                path.Data = pathGeometry;
                path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(arc._Color));
                path.StrokeThickness = arc._Thickness;
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
            EllipseGeometry newEllipse = new EllipseGeometry(new Point(pEllipse.RadiusPoint.XPosition, pEllipse.RadiusPoint.YPosition), pEllipse.Radius - pEllipse.Thickness*0.6, pEllipse.Radius - pEllipse.Thickness*0.6);
            Path path = new Path();

            path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.BorderColor));
            path.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.FillColor));
            path.StrokeThickness = pEllipse.Thickness;

            path.Data = newEllipse;

            _MainDesigner.AddShape(path);            
        }

        private void drawCustomEllipses(List<TEllipse> pEllipses)
        {
            foreach (TEllipse ellipse in pEllipses)
            {
                drawEllipse(ellipse);
            }
        }

        private void paint(TFillIndicator pFillIndicator)
        {
            pFillIndicator.adjustPosition(_MainDesigner.ActualWidth, _MainDesigner.ActualHeight); // 4 Tiempos

            Point pt = new Point(pFillIndicator.XPosition * BitmapConverter.SCALE, pFillIndicator.YPosition * BitmapConverter.SCALE); // 7 Tiempos
            Color newColor = (Color)ColorConverter.ConvertFromString(pFillIndicator.NewFillColor); // 4 Tiempos
            Color oldColor = (Color)ColorConverter.ConvertFromString(pFillIndicator.OldFillColor); // 4 Tiempos

            Stack<Point> StackPoint = new Stack<Point>(); // 3 Tiempos
            StackPoint.Push(pt); // 3 Tiempos
            while (StackPoint.Count != 0) // N
            {
                pt = StackPoint.Pop(); // 3 Tiempos
                Color CurrentColor = getPixelColor((int)pt.X, (int)pt.Y); //5 tiempos
                if (isColorMatch(CurrentColor, oldColor)) // 5 Tiempos
                {
                    Random rnd = new Random(); // 3 Tiempos
                    setPixelColor((int)pt.X, (int)pt.Y, newColor); // 4 Tiempos
                    for (int i = 0; i < 8; i++) // 1 + 8*3 Tiempos
                    {
                        StackPoint.Push(new Point(pt.X + rnd.Next(3) - 1, pt.Y + rnd.Next(3) - 1)); // 16 Tiempos
                    }
                }
            }            
        }

        /* Calculo O(n) y f(n)
         * 
         * 4 + 7 + 4 + 4 + 3 + 3 + N(3 + 5 + 5 + 3 + 4 + 1 + 8*3*16)
         * = 25 + N(21 + 8*3*16)
         * = 25 + 384N
         * 
         * f(n) = 25 + 384n
         * 
         * Duracion: O(n)
         */

        private bool isColorMatch(Color pColorA, Color pColorB)
        {
            int diff = 10;
            return (pColorA.A - diff < pColorB.A && pColorA.A + diff > pColorB.A && pColorA.R - diff < pColorB.R && pColorA.R + diff > pColorB.R &&
                        pColorA.G - diff < pColorB.G && pColorA.G + diff > pColorB.G && pColorA.B - diff < pColorB.B && pColorA.B + diff > pColorB.B);
        }

        private Color getPixelColor(int pX, int pY)
        {            
            var pixels = new byte[4];
            _MainBitmap.CopyPixels(new Int32Rect(pX,pY,1,1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void setPixelColor(int pX, int pY, Color pNewColor)
        {                                                           
            var pixels = new byte[] { pNewColor.B, pNewColor.G, pNewColor.R, pNewColor.A }; // Blue Green Red Alpha
            _MainBitmap.WritePixels(new Int32Rect(pX, pY, 1, 1), pixels, 4, 0); 
        }

        private TDesign _CurrentDesign;
        private Designer _MainDesigner;
        private Stopwatch _Watcher;
        private WriteableBitmap _MainBitmap;
        private static FireMode CurrentInstance;

        public event EventHandler<TennisEventArgs> finishDrawingDesign_EventHandler;
    }
}

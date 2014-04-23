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
    public class FireMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;
        private Stopwatch Watcher;
        private WriteableBitmap mainBitmap;
        private static FireMode _currentInstance;

        public event EventHandler<TennisEventArgs> finishDrawingDesign_EventHandler;

        public static FireMode createInstance(TDesign pDesign, Designer pDesigner)
        {
            _currentInstance = new FireMode(pDesign, pDesigner);
            return _currentInstance;
        }

        private FireMode(TDesign pDesign, Designer pDesigner)
        {
            currentDesign = pDesign;
            mainDesigner = pDesigner;
            Watcher = new Stopwatch();
        }

        public static FireMode Instance()
        {
            return _currentInstance;
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
                args.VisualizationMode = TennisEventArgs.Mode.FIRE;
                args.DesignBitmap = mainBitmap;
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
            line.StartPoint = new Point(pLine.startPoint.XPosition + TLine.POSITION_OFFSET, pLine.startPoint.YPosition + TLine.POSITION_OFFSET);
            line.EndPoint = new Point(pLine.endPoint.XPosition + TLine.POSITION_OFFSET, pLine.endPoint.YPosition + TLine.POSITION_OFFSET);

            Path path = new Path();
            path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pLine.color)); ;
            path.StrokeThickness = pLine.thickness;

            path.Data = line;
            mainDesigner.AddShape(path);
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
            EllipseGeometry newEllipse = new EllipseGeometry(new Point(pEllipse.radiusPoint.XPosition, pEllipse.radiusPoint.YPosition), pEllipse.radius, pEllipse.radius);
            Path path = new Path();

            path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.borderColor));
            path.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(pEllipse.fillColor));
            path.StrokeThickness = pEllipse.thickness;

            path.Data = newEllipse;

            mainDesigner.AddShape(path);
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
                        Random rnd = new Random();
                        SetPixelColor((int)pt.X, (int)pt.Y, newColor);
                        for (int i = 0; i < 12; i++)
                        {
                            StackPoint.Push(new Point(pt.X + rnd.Next(3) - 1, pt.Y + rnd.Next(3) - 1));
                        }
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
            mainBitmap.CopyPixels(new Int32Rect(x,y,1,1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void SetPixelColor(int x, int y, Color newColor)
        {                                                           
            var pixels = new byte[] { newColor.B, newColor.G, newColor.R, newColor.A }; // Blue Green Red Alpha
            mainBitmap.WritePixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0); 
        }
    }
}

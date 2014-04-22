using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            
            Watcher.Stop();

            EventHandler<TennisEventArgs> handler = finishDrawingDesign_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.DrawDuration = (float)Watcher.Elapsed.TotalMilliseconds;
                args.VisualizationMode = TennisEventArgs.Mode.ARCADE;
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
    }
}

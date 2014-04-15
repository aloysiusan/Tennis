using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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

        private static FireMode _currentInstance;

        public event EventHandler<TennisEventArgs> finishDrawingDesign_EventHandler;

        public static FireMode Instance()
        {
            return _currentInstance;
        }

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

        public override void initDrawing()
        {
            Watcher.Start();

            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
            this.drawCustomLines(currentDesign.customLines);

            Watcher.Stop();

            EventHandler<TennisEventArgs> handler = finishDrawingDesign_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.DrawDuration = (float)Watcher.Elapsed.TotalMilliseconds;
                args.VisualizationMode = TennisEventArgs.Mode.FIRE;
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
    }
}

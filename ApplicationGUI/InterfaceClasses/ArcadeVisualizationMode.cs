using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Tennis.Design;
using Tennis.Shapes;

namespace Tennis.ApplicationGUI
{
    public class ArcadeVisualizationMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;

        private static ArcadeVisualizationMode _currentInstance;

        public static ArcadeVisualizationMode Instance()
        {
            return _currentInstance;
        }

        public static ArcadeVisualizationMode createSingleInstance(TDesign pDesign, Designer pSender)
        {
            destroyCurrentInstance();
            _currentInstance = new ArcadeVisualizationMode(pDesign, pSender);
            return _currentInstance;
        }

        public static void destroyCurrentInstance()
        {
            _currentInstance = null;
        }

        private ArcadeVisualizationMode(TDesign pDesign, Designer pSender)
        {
            currentDesign = pDesign;
            mainDesigner = pSender;
        }

        public override void beginDrawingDesign() {
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
            this.drawCustomLines(currentDesign.customLines);
        }

        protected override void drawBorderLines(TLine[] pLines) {
            foreach (TLine tline in pLines)
            {
                drawLine(tline);
            }
        }

        protected override void drawLine(TLine pLine) {
            LineGeometry line = new LineGeometry();
            line.StartPoint = new Point(pLine.startPoint.XPosition + TLine.POSITION_OFFSET, pLine.startPoint.YPosition + TLine.POSITION_OFFSET);
            line.EndPoint = new Point(pLine.endPoint.XPosition + TLine.POSITION_OFFSET, pLine.endPoint.YPosition + TLine.POSITION_OFFSET);

            Path path = new Path();
            path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pLine.color)); ;
            path.StrokeThickness = pLine.thickness;

            path.Data = line;
            mainDesigner.AddShape(path);
        }

        protected override void drawBorderArcs(TArc[] pArcs) {
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

        protected override void drawCustomLines(List<TLine> pLines)
        {
            foreach (TLine line in pLines)
            {
                drawLine(line);
            }
        }
    }
}

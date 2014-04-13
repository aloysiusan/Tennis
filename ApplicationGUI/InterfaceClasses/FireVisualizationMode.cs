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
    class FireVisualizationMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;
        private static FireVisualizationMode _currentInstance;

        public static FireVisualizationMode Instance()
        {
            return _currentInstance;
        }

        public static FireVisualizationMode createSingleInstance(TDesign pDesign, Designer pSender)
        {
            destroyCurrentInstance();
            _currentInstance = new FireVisualizationMode(pDesign, pSender);
            return _currentInstance;
        }

        private static void destroyCurrentInstance()
        {
            _currentInstance = null;
        }

        private FireVisualizationMode(TDesign pDesign, Designer pSender) {
            currentDesign = pDesign;
            mainDesigner = pSender;
        }

        public override void beginDrawingDesign()
        {
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
        }

        protected override void drawBorderLines(TLine[] pLines)
        {
            foreach (TLine line in pLines)
            {
                drawLine(line);
            }
        }

        protected override void drawLine(TLine pLine)
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

        protected override void drawBorderArcs(TArc[] pArcs)
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
    }
}

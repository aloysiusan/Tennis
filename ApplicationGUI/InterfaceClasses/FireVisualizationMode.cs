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
            this.drawBaseLine(currentDesign.baseLine);
        }

        protected override void drawBorderLines(TLine[] pLines)
        {
            foreach (TLine line in pLines)
            {
                Line guiLine = new Line();
                guiLine.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(line.color));
                guiLine.X1 = line.startPoint.getXPosition() + TLine.POSITION_OFFSET;
                guiLine.X2 = line.endPoint.getXPosition() + TLine.POSITION_OFFSET;
                guiLine.Y1 = line.startPoint.getYPosition() + TLine.POSITION_OFFSET;
                guiLine.Y2 = line.endPoint.getYPosition() + TLine.POSITION_OFFSET;
                guiLine.StrokeThickness = line.thickness;

                mainDesigner.root.Children.Add(guiLine);
            }
        }

        protected override void drawBaseLine(TLine pLine)
        {
            Line baseLine = new Line();
            baseLine.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pLine.color));
            baseLine.X1 = pLine.startPoint.getXPosition() + TLine.POSITION_OFFSET;
            baseLine.X2 = pLine.endPoint.getXPosition() + TLine.POSITION_OFFSET;
            baseLine.Y1 = pLine.startPoint.getYPosition() + TLine.POSITION_OFFSET;
            baseLine.Y2 = pLine.endPoint.getYPosition() + TLine.POSITION_OFFSET;
            baseLine.StrokeThickness = pLine.thickness;

            mainDesigner.root.Children.Add(baseLine);
        }

        protected override void drawBorderArcs(TArc[] pArcs)
        {
            foreach (TArc arc in pArcs)
            {
                PathGeometry pathGeometry = new PathGeometry();
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(arc.startPoint.getXPosition() + TArc.POSITION_OFFSET, arc.startPoint.getYPosition() + TArc.POSITION_OFFSET);
                SweepDirection direction = !(arc.inverted) ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
                figure.Segments.Add(new ArcSegment(new Point(arc.endPoint.getXPosition() + TArc.POSITION_OFFSET, arc.endPoint.getYPosition() + TArc.POSITION_OFFSET), new Size(100, 100), 0, false, direction, true));
                pathGeometry.Figures.Add(figure);
                Path path = new Path();
                path.Data = pathGeometry;
                path.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(arc.color));
                path.StrokeThickness = arc.thickness;
                mainDesigner.root.Children.Add(path);
            }
        }
    }
}

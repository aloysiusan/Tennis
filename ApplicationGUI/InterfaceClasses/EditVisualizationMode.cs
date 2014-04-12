using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Tennis.Design;
using Tennis.Shapes;

namespace Tennis.ApplicationGUI
{
    public class EditVisualizationMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;

        private double _OffsetTop;
        private double _OffsetLeft;
        private bool _IsDragging;

        private static EditVisualizationMode _currentInstance;

        public static EditVisualizationMode Instance()
        {
            return _currentInstance;
        }

        public static EditVisualizationMode createSingleInstance(TDesign pDesign, Designer pSender)
        {
            destroyCurrentInstance();
            _currentInstance = new EditVisualizationMode(pDesign, pSender);
            return _currentInstance;
        }

        private static void destroyCurrentInstance()
        {
            _currentInstance = null;
        }

        private EditVisualizationMode(TDesign pDesign, Designer pSender) {
            currentDesign = pDesign;
            mainDesigner = pSender;
        }

        public override void beginDrawingDesign()
        {
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawBaseLine(currentDesign.baseLine);

            this.drawPoint(currentDesign.getPointWithID('a'));
            this.drawPoint(currentDesign.getPointWithID('b'));
            this.drawPoint(currentDesign.getPointWithID('c'));
            this.drawPoint(currentDesign.getPointWithID('d'));
            this.drawPoint(currentDesign.getPointWithID('e'));
        }

        protected void drawPoint(TPoint pPoint)
        {
            Ellipse guiPoint = new Ellipse();
            guiPoint.Uid = pPoint.getID().ToString();
            guiPoint.Height = TPoint.RADIUS;
            guiPoint.Width = TPoint.RADIUS;
            guiPoint.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.StrokeThickness = 1;

            guiPoint.PreviewMouseLeftButtonUp += Point_MouseLeftButtonUp;
            guiPoint.MouseMove += Point_MouseMove;
            guiPoint.MouseLeftButtonDown += Point_MouseLeftButtonDown;

            Canvas.SetLeft(guiPoint, pPoint.getXPosition());
            Canvas.SetTop(guiPoint, pPoint.getYPosition());
            mainDesigner.root.Children.Add(guiPoint);
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

        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                Point p = e.GetPosition(mainDesigner);
                if (p.X >= 0 && p.X <= mainDesigner.root.ActualWidth
                    && p.Y >= 0 && p.Y <= mainDesigner.root.ActualHeight)
                {
                    currentDesign.getPointWithID(selectedPoint.Uid[0]).setXPositionRelative(p.X, mainDesigner.ActualWidth);
                    currentDesign.getPointWithID(selectedPoint.Uid[0]).setYPositionRelative(p.Y, mainDesigner.ActualHeight);
                }
                selectedPoint.ReleaseMouseCapture();
                Canvas.SetZIndex(selectedPoint, 0);
                _IsDragging = false;
                selectedPoint = null;
                mainDesigner.root.Children.Clear();
                this.beginDrawingDesign();
            }
        }

        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsDragging)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                Point p = e.GetPosition(mainDesigner);

                if (p.X >= 0 && p.X <= mainDesigner.root.ActualWidth
                    && p.Y >= 0 && p.Y <= mainDesigner.root.ActualHeight)
                {
                    Canvas.SetLeft(selectedPoint, p.X + _OffsetLeft);
                    Canvas.SetTop(selectedPoint, p.Y + _OffsetTop);
                }
            }
        }

        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                selectedPoint.CaptureMouse();
                _IsDragging = true;
                Point p = e.GetPosition(mainDesigner);
                _OffsetTop = Canvas.GetTop(selectedPoint) - p.Y;
                _OffsetLeft = Canvas.GetLeft(selectedPoint) - p.X;
                Canvas.SetZIndex(selectedPoint, 5);
                selectedPoint = null;
            }
        }
    }
}

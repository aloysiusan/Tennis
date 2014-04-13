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

        public bool isDrawingLines;
        public int newLinesThickness;
        public string newLinesColor;

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
            mainDesigner.MouseLeftButtonDown += mainDesigner_MouseLeftButtonDown;
            mainDesigner.MouseLeftButtonUp +=mainDesigner_MouseLeftButtonUp;
            mainDesigner.MouseMove +=mainDesigner_MouseMove;
        }

        public override void beginDrawingDesign()
        {
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
            this.drawCustomLines(currentDesign.customLines);

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

            guiPoint.MouseLeftButtonUp += Point_MouseLeftButtonUp;
            guiPoint.MouseMove += Point_MouseMove;
            guiPoint.MouseLeftButtonDown += Point_MouseLeftButtonDown;

            Canvas.SetLeft(guiPoint, pPoint.XPosition);
            Canvas.SetTop(guiPoint, pPoint.YPosition);
            mainDesigner.AddShape(guiPoint);
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

        protected override void drawCustomLines(List<TLine> pLines)
        {
            foreach (TLine line in pLines)
            {
                drawLine(line);
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

        Line newLine;
        private void mainDesigner_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingLines)
            {
                _IsDragging = true;
                newLine = new Line();
                newLine.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(newLinesColor));
                Point p = e.GetPosition(mainDesigner);
                _OffsetTop = p.Y;
                _OffsetLeft = p.X;
                newLine.X1 = newLine.X2 = _OffsetLeft;
                newLine.Y1 = newLine.Y2 = _OffsetTop;
                newLine.StrokeThickness = newLinesThickness;
                mainDesigner.AddShape(newLine);
            }
        }

        private void mainDesigner_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingLines && _IsDragging)
            {
                _IsDragging = false;
                TPoint startPoint = new TPoint('l', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
                startPoint.loadCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, newLine.X1, newLine.Y1);

                TPoint endPoint = new TPoint('l', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
                endPoint.loadCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, newLine.X2, newLine.Y2);

                TLine newTLine = new TLine(startPoint, endPoint);
                newTLine.color = newLinesColor;
                newTLine.thickness = newLinesThickness;
                currentDesign.customLines.Add(newTLine);
                newLine = null;
            }
        }

        private void mainDesigner_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingLines && _IsDragging)
            {
                Point p = e.GetPosition(mainDesigner);
                newLine.X2 = p.X;
                newLine.Y2 = p.Y;
            }
        }
    }
}

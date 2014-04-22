using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tennis.Design;
using Tennis.Shapes;

namespace Tennis.ApplicationGUI
{
    public class EditMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;

        private double _OffsetTop;
        private double _OffsetLeft;
        private bool _IsDragging;

        public bool isDrawingLines;
        public int newLinesThickness;
        public string newLinesColor;

        public bool isDrawingEllipse;
        public int ellipseThickness;
        public int ellipseRadius;
        public string ellipseBorderColor = "#000000";
        public string ellipseFillColor = "#000000";

        public string paintColor = "#000000";
        public bool isPainting;

        private static EditMode _currentInstance;

        public static EditMode Instance()
        {
            return _currentInstance;
        }

        public static EditMode createInstance(TDesign pDesign, Designer pSender)
        {
            _currentInstance = new EditMode(pDesign, pSender);
            return _currentInstance;
        }


        private EditMode(TDesign pDesign, Designer pSender) {
            currentDesign = pDesign;
            mainDesigner = pSender;
            mainDesigner.MouseLeftButtonDown += mainDesigner_MouseLeftButtonDown;
            mainDesigner.MouseLeftButtonUp +=mainDesigner_MouseLeftButtonUp;
            mainDesigner.MouseMove +=mainDesigner_MouseMove;
        }

        public override void initDrawing()
        {
            mainDesigner.root.Children.Clear();
            this.drawBorderLines(currentDesign.designLines);
            this.drawBorderArcs(currentDesign.designArcs);
            this.drawLine(currentDesign.baseLine);
            this.drawCustomLines(currentDesign.customLines);
            this.drawCustomEllipses(currentDesign.customEllipses);

            this.drawPoint(currentDesign.getPointWithID('a'));
            this.drawPoint(currentDesign.getPointWithID('b'));
            this.drawPoint(currentDesign.getPointWithID('c'));
            this.drawPoint(currentDesign.getPointWithID('d'));
            this.drawPoint(currentDesign.getPointWithID('e'));

            this.drawFillPoints(currentDesign.fillIndicators);

        }

        private void drawPoint(TPoint pPoint)
        {
            Ellipse guiPoint = new Ellipse();
            guiPoint.Uid = pPoint.getID().ToString();
            guiPoint.Height = TPoint.RADIUS;
            guiPoint.Width = TPoint.RADIUS;
            guiPoint.Fill = (pPoint.getID() == 'f') ? (SolidColorBrush)(new BrushConverter().ConvertFrom(pPoint.fillColor)) : (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.DEFAULT_COLOR));
            guiPoint.Stroke = (pPoint.getID() == 'f') ? (SolidColorBrush)(new BrushConverter().ConvertFrom(pPoint.fillColor)) : (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.DEFAULT_COLOR));
            guiPoint.StrokeThickness = 1;
            guiPoint.Cursor = Cursors.Hand;

            guiPoint.MouseLeftButtonUp += Point_MouseLeftButtonUp;
            guiPoint.MouseMove += Point_MouseMove;
            guiPoint.MouseLeftButtonDown += Point_MouseLeftButtonDown;

            Canvas.SetLeft(guiPoint, pPoint.XPosition);
            Canvas.SetTop(guiPoint, pPoint.YPosition);
            mainDesigner.AddShape(guiPoint);
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

        private void drawCustomEllipses(List<TEllipse> pEllipses)
        {
            foreach (TEllipse ellipse in pEllipses)
            {
                drawEllipse(ellipse);
            }
        }

        private void drawCustomLine(double pStartX, double pStartY, double pEndX, double pEndY)
        {
            TPoint startPoint = new TPoint('l', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
            startPoint.setCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, pStartX, pStartY);

            TPoint endPoint = new TPoint('l', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
            endPoint.setCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, pEndX, pEndY);

            TLine newTLine = new TLine(startPoint, endPoint);
            newTLine.color = newLinesColor;
            newTLine.thickness = newLinesThickness;
            currentDesign.customLines.Add(newTLine);
            newLine = null;
        }

        private void drawEllipse(TEllipse pElipse) 
        {
            Ellipse newEllipse = new Ellipse();
            newEllipse.Height = pElipse.radius*2;
            newEllipse.Width = pElipse.radius*2;
            newEllipse.StrokeThickness = pElipse.thickness;
            newEllipse.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(pElipse.fillColor));
            newEllipse.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(pElipse.borderColor));
            Canvas.SetLeft(newEllipse, pElipse.radiusPoint.XPosition);
            Canvas.SetTop(newEllipse, pElipse.radiusPoint.YPosition);
            mainDesigner.AddShape(newEllipse);

        }

        private void createFillPoint(Point p)
        {
            TPoint fillPoint = new TPoint('f', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
            fillPoint.setCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, p.X, p.Y);
            fillPoint.fillColor = paintColor;
            Color oldColor = GetPixelColor(BitmapConverter.CreateWriteableBitmapFromCanvas(mainDesigner.root), (int)p.X, (int)p.Y);
            fillPoint.oldColor = "#" + oldColor.R.ToString("X2") + oldColor.G.ToString("X2") + oldColor.B.ToString("X2");
            currentDesign.fillIndicators.Add(fillPoint);
            drawPoint(fillPoint);
        }

        private void drawFillPoints(List<TPoint> pPoints)
        {
            foreach (TPoint p in pPoints)
            {
                drawPoint(p);
            }
        }

        public void setBorderThickness(int pValue)
        {
            currentDesign.setBorderThickness(pValue);
            this.initDrawing();
        }

        public void setBaseLineThickness(int pValue)
        {
            currentDesign.setBaseLineThickness(pValue);
            this.initDrawing();
        }

        public void setBorderColor(String pColor)
        {
            currentDesign.setBorderColor(pColor);
            this.initDrawing();
        }

        public void setBaseLineColor(String pColor)
        {
            currentDesign.setBaseLineColor(pColor);
            this.initDrawing();

        }

        private Color GetPixelColor(WriteableBitmap pBitmap, int x, int y)
        {
            var pixels = new byte[4];
            pBitmap.CopyPixels(new Int32Rect(x, y, 1, 1), pixels, 4, 0);
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null && !isDrawingLines)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                if(selectedPoint.Uid != "f"){
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
                this.initDrawing();
                }                
            }
        }

        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsDragging && !isDrawingLines)
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
            if (sender != null && !isDrawingLines)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                if (selectedPoint.Uid != "f")
                {
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
                drawCustomLine(newLine.X1, newLine.Y1, newLine.X2, newLine.Y2);
            }

            if (isDrawingEllipse)
            {
                TPoint ellipseCenter = new TPoint('r', mainDesigner.ActualWidth, mainDesigner.ActualHeight, TPoint.DefaultPosition.DefaultGeneric_XPosition, TPoint.DefaultPosition.DefaultGeneric_YPosition);
                ellipseCenter.setCustomPointData(mainDesigner.ActualWidth, mainDesigner.ActualHeight, e.GetPosition(mainDesigner).X, e.GetPosition(mainDesigner).Y);

                TEllipse newEllipse = new TEllipse(ellipseCenter);
                newEllipse.radius = ellipseRadius;
                newEllipse.thickness = ellipseThickness;
                newEllipse.fillColor = ellipseFillColor;
                newEllipse.borderColor = ellipseBorderColor;
                currentDesign.customEllipses.Add(newEllipse);

                drawEllipse(newEllipse);
                mainDesigner.Cursor = Cursors.Arrow;
                isDrawingEllipse = false;
            }

            if (isPainting)
            {               
                this.createFillPoint(e.GetPosition(mainDesigner));
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

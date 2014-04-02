using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tennis.Shapes;
using Tennis.Design;
using Tennis.Controllers;
namespace Tennis.ApplicationGUI
{
	/// <summary>
	/// Lógica de interacción para Designer.xaml
	/// </summary>
    /// 

    public partial class Designer : UserControl
	{
        private double _OffsetTop;
        private double _OffsetLeft;
        private bool _IsDragging;

		public Designer()
		{
            this.InitializeComponent();
		}

        public void drawDefaultDesignUsingID(String pID)
        {
            this.root.Children.Clear();
            AppMainController.Instance().setCurrentDesign(new TDesign(pID,this.ActualWidth, this.ActualHeight));
            this.drawDesign(AppMainController.Instance().getCurrentDesign());
        }

        public void drawDesign(TDesign pDesign)
        {
            this.drawDesignLines(pDesign);
            this.drawDesignArcs(pDesign);

            this.drawDesignPoint(pDesign.getPointWithID('a'));
            this.drawDesignPoint(pDesign.getPointWithID('b'));
            this.drawDesignPoint(pDesign.getPointWithID('c'));
            this.drawDesignPoint(pDesign.getPointWithID('d'));
            this.drawDesignPoint(pDesign.getPointWithID('e'));
        }

        private void drawDesignPoint(TPoint pPoint)
        {
            Ellipse guiPoint = new Ellipse();
            guiPoint.Uid = pPoint.getID().ToString();
            guiPoint.Height = TPoint.RADIUS;
            guiPoint.Width = TPoint.RADIUS;
            guiPoint.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.StrokeThickness = 1;

            guiPoint.PreviewMouseLeftButtonUp +=guiPoint_MouseLeftButtonUp;
            guiPoint.MouseMove +=guiPoint_MouseMove;
            guiPoint.MouseLeftButtonDown +=guiPoint_MouseLeftButtonDown;

            Canvas.SetLeft(guiPoint, pPoint.getXPosition());
            Canvas.SetTop(guiPoint, pPoint.getYPosition());
            this.root.Children.Add(guiPoint);
        }

        private void drawDesignLines(TDesign pDesign)
        {
            foreach(TLine line in pDesign.getDesignLines()){
                Line guiLine = new Line();
                guiLine.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(line.color));
                guiLine.X1 = line.startPoint.getXPosition() + TLine.POSITION_OFFSET;
                guiLine.X2 = line.endPoint.getXPosition() + TLine.POSITION_OFFSET;
                guiLine.Y1 = line.startPoint.getYPosition() + TLine.POSITION_OFFSET;
                guiLine.Y2 = line.endPoint.getYPosition() + TLine.POSITION_OFFSET;
                guiLine.StrokeThickness = line.thickness;
                
                this.root.Children.Add(guiLine);
            }
        }

        private void drawDesignArcs(TDesign pDesign)
        {
            foreach (TArc arc in pDesign.getDesignArcs())
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
                this.root.Children.Add(path);
            }
        }

        void guiPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                Point p = e.GetPosition(this);
                if (p.X >= 0 && p.X <= this.root.ActualWidth
                    && p.Y >= 0 && p.Y <= this.root.ActualHeight)
                {
                    AppMainController.Instance().getCurrentDesign().getPointWithID(selectedPoint.Uid[0]).setXPosition(p.X);
                    AppMainController.Instance().getCurrentDesign().getPointWithID(selectedPoint.Uid[0]).setYPosition(p.Y);
                }
                selectedPoint.ReleaseMouseCapture();
                Canvas.SetZIndex(selectedPoint, 0);
                _IsDragging = false;
                selectedPoint = null;
                this.root.Children.Clear();
                this.drawDesign(AppMainController.Instance().getCurrentDesign());
            }
        }

        void guiPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (_IsDragging)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                Point p = e.GetPosition(this);
                
                if (p.X >= 0 && p.X <= this.root.ActualWidth
                    && p.Y >= 0 && p.Y <= this.root.ActualHeight)
                {                    
                    Canvas.SetLeft(selectedPoint, p.X + _OffsetLeft);
                    Canvas.SetTop(selectedPoint, p.Y + _OffsetTop);
                }
            }
        }

        void guiPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                Ellipse selectedPoint = ((Ellipse)sender);
                selectedPoint.CaptureMouse();
                _IsDragging = true;
                Point p = e.GetPosition(this);
                _OffsetTop = Canvas.GetTop(selectedPoint) - p.Y;
                _OffsetLeft = Canvas.GetLeft(selectedPoint) - p.X;
                Canvas.SetZIndex(selectedPoint, 5);
                selectedPoint = null;
            }            
        }

	}
}
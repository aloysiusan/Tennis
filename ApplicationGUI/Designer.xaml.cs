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

namespace Tennis.ApplicationGUI
{
	/// <summary>
	/// Lógica de interacción para Designer.xaml
	/// </summary>
    /// 

    public partial class Designer : UserControl
	{       
		public Designer()
		{
            this.InitializeComponent();
		}

        public void drawDefaultDesign()
        {
            TDesign newDesign = new TDesign(this.ActualWidth, this.ActualHeight);
            this.drawDesign(newDesign);
        }

        public void drawDesign(TDesign pDesign)
        {
            this.drawDesignPoint(pDesign.getPointA());
            this.drawDesignPoint(pDesign.getPointB());
            this.drawDesignPoint(pDesign.getPointC());
            this.drawDesignPoint(pDesign.getPointD());
            this.drawDesignPoint(pDesign.getPointE());
        }

        private void drawDesignPoint(TPoint pPoint)
        {
            Ellipse guiPoint = new Ellipse();
            guiPoint.Uid = pPoint.getID();
            guiPoint.Height = TPoint.RADIUS;
            guiPoint.Width = TPoint.RADIUS;
            guiPoint.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.Stroke = (SolidColorBrush)(new BrushConverter().ConvertFrom(TPoint.COLOR));
            guiPoint.StrokeThickness = 1;
            guiPoint.MouseLeftButtonUp +=guiPoint_MouseLeftButtonUp;
            guiPoint.MouseMove +=guiPoint_MouseMove;
            guiPoint.MouseLeftButtonDown +=guiPoint_MouseLeftButtonDown;
            Canvas.SetLeft(guiPoint, pPoint.getXPosition());
            Canvas.SetTop(guiPoint, pPoint.getYPosition());
            this.root.Children.Add(guiPoint);
        }

        private void drawDesignLine(TDesign pDesign)
        {

        }

        private void drawDesignArc(TDesign pDesign)
        {

        }

        void guiPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        void guiPoint_MouseMove(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        void guiPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

	}
}
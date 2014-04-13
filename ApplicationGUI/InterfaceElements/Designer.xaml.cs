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
using Tennis.ApplicationLogic;

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

        public void AddShape(UIElement pShape)
        {
            this.root.Children.Add(pShape);
        }
	}
}
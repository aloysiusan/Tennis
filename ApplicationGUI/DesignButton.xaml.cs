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

namespace Tennis.ApplicationGUI
{
	/// <summary>
	/// Lógica de interacción para DesignButton.xaml
	/// </summary>
	public partial class DesignButton : UserControl
	{
		public DesignButton(String pName, String pCreationDate)
		{
			this.InitializeComponent();
            lblName.Content = pName;
            lblCreationDate.Content = "Creado: " + pCreationDate;
		}
	}
}
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
	/// Lógica de interacción para Designer.xaml
	/// </summary>
    /// 

    /* Porcentaje de localización inicial X de los puntos en el Canvas:
     * A = 10%
     * B = 40%
     * C = 60%
     * D = 90%
     * E = 10%
     */

    /* Porcentaje de localización inicial Y de los puntos en el Canvas:
    * A = 20%
    * B = 10%
    * C = 50%
    * D = 90%
    * E = 90%
    */

    public partial class Designer : UserControl
	{

		public Designer()
		{
            this.InitializeComponent();
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
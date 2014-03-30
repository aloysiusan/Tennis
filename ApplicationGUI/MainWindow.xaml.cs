using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tennis.LogicController;

namespace Tennis.ApplicationGUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TGUIController.Instance().paintDesignWithID(0);
        }
        
        private void btnNewDesign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!insertNameMessageView.IsVisible)
            {                              
                insertNameMessageView.Visibility = Visibility.Visible;
                txtNewName.Focus();
            }
        }

        private void txtNewName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                insertNameMessageView.Visibility = Visibility.Hidden;
                txtNewName.Clear();
            }

            if (e.Key == Key.Enter && txtNewName.Text != "")
            {
                List<object> results = TGUIController.Instance().allDesignsList();
               
                for (int i = 0; i < results.Count; i++)
                {                    
                    Dictionary<string, string[]> design = (Dictionary<string, string[]>)results.ElementAt(i);
                    String key = (design.Keys.ElementAt(0));
                    string[] info;
                    design.TryGetValue(key,out info);
                    Debug.Print(design.Keys.ElementAt(0) + " - " + info[0] + " - " + info[1]);
                }
            }
        }

        private void lineThicknessSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           //lineThicknessLbl.Content = e.NewValue.ToString();
            lineObject.StrokeThickness = e.NewValue;
        }

    }
}

using Microsoft.Expression.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tennis.Controllers;
using Tennis.TEventArgs;

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
            
            AppMainController.Instance().designsReady_EventHandler += new EventHandler<TennisEventArgs>(this.fillListWithDesigns);
            AppMainController.Instance().designCreationStatusReady_EventHandler += new EventHandler<TennisEventArgs>(this.createDesign);
        
        }
        
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
            AppMainController.Instance().requestDesignsFromDataBase();
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
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
                AppMainController.Instance().requestNewDesignCreation(txtNewName.Text);
                insertNameMessageView.Visibility = Visibility.Hidden;
                txtNewName.Clear();
                btnNewDesign.IsEnabled = false;
            }
        }

        private void lineThicknessSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
        }

        private void fillListWithDesigns(object sender, TennisEventArgs args) {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            if (args.FinishedSuccessfully)
            {
                foreach (Dictionary<string, string[]> obj in args.ParseData)
                {
                    foreach (var entry in obj)
                    {
                        Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton(entry.Key, entry.Value[0], entry.Value[1], false))));
                    }
                }
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar cargar los diseños. Por favor verifique su conexión y vuelva a intentarlo.","Error al Conectar",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
        }

        private void createDesign(object sender, TennisEventArgs args)
        {
            if (args.FinishedSuccessfully)
            {
                //Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Clear()));                
                //AppMainController.Instance().requestDesignsFromDataBase();
                Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
                //Dispatcher.BeginInvoke(new Action(() => DesignButton.getInstances()[DesignButton.getInstances().Count - 1].setSelected()));     
               // Dispatcher.BeginInvoke(new Action(() => Console.WriteLine(DesignButton.getInstances().Count)));
                Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton("test", "test", "test", true))));
                Dispatcher.BeginInvoke(new Action(() => designerView.drawDefaultDesign()));
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el nuevo diseño.", "Error Inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

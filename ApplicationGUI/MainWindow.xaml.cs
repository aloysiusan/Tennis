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
using Tennis.Parse.Rows;

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
            AppMainController.Instance().designCreationStatusFailed_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFailed);
            AppMainController.Instance().designDataReady_EventHandler += new EventHandler<TennisEventArgs>(this.loadCurrentDesign);
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
                foreach (string[] obj in args.DesignsList)
                {
                    Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton(obj[0], obj[1], obj[2], false, OnDesignItemSelected))));                    
                }
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar cargar los diseños. Por favor verifique su conexión y vuelva a intentarlo.","Error al Conectar",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
        }

        private void OnDesignCreationFailed(object sender, TennisEventArgs args)
        {            
            if (!args.FinishedSuccessfully)
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el nuevo diseño.", "Error Inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnDesignItemSelected(object sender, TennisEventArgs args)
        {
            if (args.IsNewDesign)
            {
                AppMainController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                Dispatcher.BeginInvoke(new Action(() => designerView.drawDesign(AppMainController.Instance().getCurrentDesign())));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
                AppMainController.Instance().requestDesignForID(args.SelectedDesignID, false);
            }
        }

        private void loadCurrentDesign(object sender, TennisEventArgs args)
        {            
            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));

            if (args.FinishedSuccessfully && args.IsNewDesign)
            {
                Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton((string)args.DesignData[0], (string)args.DesignData[1], 
                    (string)args.DesignData[2], true, OnDesignItemSelected))));
                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));                
            }
            else
            {                
                AppMainController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                Dispatcher.BeginInvoke(new Action(() => designerView.drawDesign(AppMainController.Instance().getCurrentDesign())));
                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                //var dump = ObjectDumper.Dump(AppMainController.Instance().getCurrentDesign());
                //.Write(dump);
            }
        }
    }
}

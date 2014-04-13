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
using Tennis.ApplicationLogic;
using Tennis.TEventArgs;
using Tennis.Parse.Rows;

namespace Tennis.ApplicationGUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum Mode
        {
            FIRE, ARCADE
        }

        private Mode selectedMode;        

        public MainWindow()
        {
            InitializeComponent();

            selectedMode = Mode.FIRE;
            ApplicationController.Instance().designsReady_EventHandler += new EventHandler<TennisEventArgs>(this.fillListWithDesigns);
            ApplicationController.Instance().designCreationStatusFailed_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFailed);
            ApplicationController.Instance().designDataReady_EventHandler += new EventHandler<TennisEventArgs>(this.loadSelectedDesign);
        }
        
        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = true));
            ApplicationController.Instance().requestDesignsFromDataBase();
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
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = true));
                ApplicationController.Instance().requestNewDesignCreation(txtNewName.Text);
                insertNameMessageView.Visibility = Visibility.Hidden;
                txtNewName.Clear();
                btnNewDesign.IsEnabled = false;
            }
        }

        public void drawDesignUsingMode(VisualizationMode pMode)
        {
            designerView.root.Children.Clear();
            designerView.BorderThickness = new Thickness(1);

            pMode.beginDrawingDesign();
        } 
        
        private void fillListWithDesigns(object sender, TennisEventArgs args) {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = false));
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
                ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                Dispatcher.BeginInvoke(new Action(() => beginEditingMode()));
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = true));
                ApplicationController.Instance().requestDesignForID(args.SelectedDesignID, false);
            }
        }

               
        private void loadSelectedDesign(object sender, TennisEventArgs args)
        {            
            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = false));

            if (args.FinishedSuccessfully && args.IsNewDesign)
            {
                Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton((string)args.DesignData[0], (string)args.DesignData[1], 
                    (string)args.DesignData[2], true, OnDesignItemSelected))));
                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));                
            }
            else
            {                
                ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                if(selectedMode == Mode.FIRE) 
                    Dispatcher.BeginInvoke(new Action(() => this.drawDesignUsingMode(FireVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView))));                
                else
                    Dispatcher.BeginInvoke(new Action(() => this.drawDesignUsingMode(ArcadeVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView))));

                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                Dispatcher.BeginInvoke(new Action(() => btnEditDesign.IsEnabled = true));

                //var dump = ObjectDumper.Dump(AppMainController.Instance().getCurrentDesign());
                //.Write(dump);
            }
        }

        private void rdbDrawFire_Checked(object sender, RoutedEventArgs e)
        {
            selectedMode = Mode.FIRE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
            {
                ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                this.drawDesignUsingMode(FireVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
            }
        }

        private void rdbDrawArcade_Checked(object sender, RoutedEventArgs e)
        {
            selectedMode = Mode.ARCADE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
            {
                ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
                this.drawDesignUsingMode(ArcadeVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
            }
        }

        private void btnSaveDesign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            beginViewMode();
        }

        private void btnEditDesign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            beginEditingMode();
        }

        public void beginEditingMode()
        {
            btnSaveDesign.IsEnabled = true;
            btnEditDesign.IsEnabled = false;
            btnSaveDesign.Visibility = Visibility.Visible;
            settingsView.Width = 220;
            settingsView.IsEnabled = true;
            rdbDrawArcade.IsEnabled = false;
            rdbDrawFire.IsEnabled = false;
            designerContainer.Margin = new Thickness(220, 0, 220, 0);
            desingsList.IsEnabled = false;
            btnNewDesign.IsEnabled = false;
            ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
            this.drawDesignUsingMode(EditVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
        }

        public void beginViewMode()
        {
            btnSaveDesign.IsEnabled = false;
            btnEditDesign.IsEnabled = true;
            btnSaveDesign.Visibility = Visibility.Hidden;
            settingsView.Width = 0;
            settingsView.IsEnabled = false;
            designerContainer.Margin = new Thickness(220, 0, 0, 0);
            rdbDrawArcade.IsEnabled = true;
            rdbDrawFire.IsEnabled = true;
            desingsList.IsEnabled = true;
            btnNewDesign.IsEnabled = true;
            ApplicationController.Instance().saveCurrentDesign();
            ApplicationController.Instance().getCurrentDesign().adjustPoints(this.designerView.ActualWidth, this.designerView.ActualHeight);
            chkDrawLines.IsChecked = false;
            if (selectedMode == Mode.FIRE)
                Dispatcher.BeginInvoke(new Action(() => this.drawDesignUsingMode(FireVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView))));
            else
                Dispatcher.BeginInvoke(new Action(() => this.drawDesignUsingMode(ArcadeVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView))));

        }

        private void lineThicknessSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try{ 
                lineThicknessLbl.Content = ((int)e.NewValue).ToString() + "px";
                EditVisualizationMode.Instance().newLinesThickness = (int)e.NewValue;
            }
            catch (NullReferenceException) { }            
        }

        private void sldBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try{ 
                lblBorderThickness.Content = ((int)e.NewValue).ToString() + "px";
                ApplicationController.Instance().getCurrentDesign().setBorderThickness((int)e.NewValue);
                this.drawDesignUsingMode(EditVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
            }
            catch (NullReferenceException) { }            
        }

        private void sldBaseLineThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblBaseLineThickness.Content = ((int)e.NewValue).ToString() + "px";
                ApplicationController.Instance().getCurrentDesign().setBaseLineThickness((int)e.NewValue);
                this.drawDesignUsingMode(EditVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
            }
            catch (NullReferenceException) { }    
        }

        private void borderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            ApplicationController.Instance().getCurrentDesign().setBorderColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));
            this.drawDesignUsingMode(EditVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
        }

        private void baseLineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            ApplicationController.Instance().getCurrentDesign().setBaseLineColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));
            this.drawDesignUsingMode(EditVisualizationMode.createSingleInstance(ApplicationController.Instance().getCurrentDesign(), designerView));
        }
        
        private void lineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditVisualizationMode.Instance().newLinesColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }   

        private void chkDrawLines_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Hand;
            EditVisualizationMode.Instance().isDrawingLines = true;
            EditVisualizationMode.Instance().newLinesColor = "#" + lineColorPicker.SelectedColor.R.ToString("X2") + lineColorPicker.SelectedColor.G.ToString("X2") + lineColorPicker.SelectedColor.B.ToString("X2");
            EditVisualizationMode.Instance().newLinesThickness = (int)lineThicknessSld.Value;
        }
        
        private void chkDrawLines_Unchecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Arrow;
            EditVisualizationMode.Instance().isDrawingLines = false;
        }   

    }
}

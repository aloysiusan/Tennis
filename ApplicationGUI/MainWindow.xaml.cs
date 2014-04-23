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
using System.Threading.Tasks;

namespace Tennis.ApplicationGUI
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private VisualizationMode.Mode selectedMode;        

        public MainWindow()
        {
            InitializeComponent();

            selectedMode = VisualizationMode.Mode.FIRE;
            ApplicationController.Instance().designsReady_EventHandler += new EventHandler<TennisEventArgs>(this.fillListWithDesigns);
            ApplicationController.Instance().designCreationStatusFailed_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFailed);
            ApplicationController.Instance().designDataReady_EventHandler += new EventHandler<TennisEventArgs>(this.loadSelectedDesign);
            ApplicationController.Instance().designDurationUpdated_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignFinishedLoading);
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

        private void drawDesignUsingMode(VisualizationMode pMode)
        {
            Thread thread = new Thread(pMode.initDrawing());
            thread.Start();
            
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
                Dispatcher.BeginInvoke(new Action(() => beginEditingMode()));            
            else
            {
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
                Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = true));
                ApplicationController.Instance().requestDesignForID(args.SelectedDesignID, false);
            }
        }

               
        private void loadSelectedDesign(object sender, TennisEventArgs args)
        {
            if (args.FinishedSuccessfully && args.IsNewDesign)
            {
                Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton((string)args.DesignData[0], (string)args.DesignData[1], 
                    (string)args.DesignData[2], true, OnDesignItemSelected))));
                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                Dispatcher.BeginInvoke(new Action(() => currentDesignReportsView.Visibility = Visibility.Hidden));  
            }
            else
            {                
                this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(),this.designerView,selectedMode));                

                Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                Dispatcher.BeginInvoke(new Action(() => btnEditDesign.IsEnabled = true));               
            }

            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = false));
            Dispatcher.BeginInvoke(new Action(() => btnReports.IsEnabled = true));           
        }

        private void rdbDrawFire_Checked(object sender, RoutedEventArgs e)
        {
            selectedMode = VisualizationMode.Mode.FIRE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
                this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode));    
            
        }

        private void rdbDrawArcade_Checked(object sender, RoutedEventArgs e)
        {
            selectedMode = VisualizationMode.Mode.ARCADE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
                this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode)); 
            
        }

        private void btnSaveDesign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            beginViewModeSaving(true);
        }

        private void btnEditDesign_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            beginEditingMode();
        }


        private void btnCancelEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            beginViewModeSaving(false);
        } 

        public void beginEditingMode()
        {
            btnSaveDesign.IsEnabled = true;
            btnEditDesign.IsEnabled = false;
            btnCancelEdit.IsEnabled = true;
            btnCancelEdit.Visibility = Visibility.Visible;
            btnSaveDesign.Visibility = Visibility.Visible;
            settingsView.Width = 220;
            settingsView.IsEnabled = true;
            rdbDrawArcade.IsEnabled = false;
            rdbDrawFire.IsEnabled = false;
            designerContainer.Margin = new Thickness(220, 0, 220, 0);
            desingsList.IsEnabled = false;
            btnNewDesign.IsEnabled = false;
            this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), designerView, VisualizationMode.Mode.EDIT));
        }

        public void beginViewModeSaving(bool pSaving)
        {
            btnSaveDesign.IsEnabled = false;
            btnEditDesign.IsEnabled = true;
            btnCancelEdit.IsEnabled = false;
            btnCancelEdit.Visibility = Visibility.Hidden;
            btnSaveDesign.Visibility = Visibility.Hidden;
            settingsView.Width = 0;
            settingsView.IsEnabled = false;
            designerContainer.Margin = new Thickness(220, 0, 0, 0);
            rdbDrawArcade.IsEnabled = true;
            rdbDrawFire.IsEnabled = true;
            desingsList.IsEnabled = true;
            btnNewDesign.IsEnabled = true;

            if (pSaving)
            {
                ApplicationController.Instance().saveCurrentDesign();
            }

            chkDrawLines.IsChecked = false;
            chkPaint.IsChecked = false;

            this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode)); 

        }

        private void lineThicknessSld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try{ 
                lineThicknessLbl.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().newLinesThickness = (int)e.NewValue;
            }
            catch (NullReferenceException) { }            
        }

        private void sldBorderThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try{ 
                lblBorderThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().setBorderThickness((int)e.NewValue);
            }
            catch (NullReferenceException) { }            
        }

        private void sldBaseLineThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblBaseLineThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().setBaseLineThickness((int)e.NewValue);
            }
            catch (NullReferenceException) { }    
        }

        private void borderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().setBorderColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));            
        }

        private void baseLineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().setBaseLineColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));
        }
        
        private void lineColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().newLinesColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }   

        private void chkDrawLines_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Hand;
            chkPaint.IsChecked = false;
            EditMode.Instance().isDrawingLines = true;
            EditMode.Instance().newLinesColor = "#" + lineColorPicker.SelectedColor.R.ToString("X2") + lineColorPicker.SelectedColor.G.ToString("X2") + lineColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().newLinesThickness = (int)lineThicknessSld.Value;
        }
        
        private void chkDrawLines_Unchecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Arrow;
            EditMode.Instance().isDrawingLines = false;
        }

        private void btnReports_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (!currentDesignReportsView.IsVisible){                
                currentDesignReportsView.Visibility = Visibility.Visible;
                currentDesignReportsView.Focus();
            }
			else{
				currentDesignReportsView.Visibility = Visibility.Hidden;
			}
        }

        private void Element_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
			insertNameMessageView.Visibility = Visibility.Hidden;
            txtNewName.Clear();
        	currentDesignReportsView.Visibility = Visibility.Hidden; 
        }

        private void OnDesignFinishedLoading(object sender, TennisEventArgs args)
        {
            lblFireDuration.Content = ApplicationController.Instance().getCurrentDesignFireDuration();
            lblFireDate.Content = ApplicationController.Instance().getCurrentDesignFireDurationDate();
            lblArcadeDuration.Content = (ApplicationController.Instance().getCurrentDesignArcadeDuration());
            lblArcadeDate.Content = ApplicationController.Instance().getCurrentDesignArcadeDurationDate();

            Console.WriteLine("finished drawing");
            designerView.root.Children.Clear();
            Image finishedDesign = new Image();
            finishedDesign.Source = args.DesignBitmap;
            designerView.AddShape(finishedDesign);
        }

        private void btnDrawShape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditMode.Instance().isDrawingEllipse = true;
            designerView.Cursor = Cursors.Hand;
            EditMode.Instance().ellipseBorderColor = "#" + shapeBorderColorPicker.SelectedColor.R.ToString("X2") + shapeBorderColorPicker.SelectedColor.G.ToString("X2") + shapeBorderColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().ellipseFillColor = "#" + shapeFillColorPicker.SelectedColor.R.ToString("X2") + shapeFillColorPicker.SelectedColor.G.ToString("X2") + shapeFillColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().ellipseRadius = (int)sldEllipseRadius.Value;
            EditMode.Instance().ellipseThickness = (int)sldEllipseThickness.Value;
        }

        private void sldEllipseThickness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblEllipseThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().ellipseThickness = ((int)e.NewValue);
            }
            catch (NullReferenceException) { } 
        }

        private void sldEllipseRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblEllipseRadius.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().ellipseRadius = ((int)e.NewValue);
            }
            catch (NullReferenceException) { } 
        }

        private void shapeBorderColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().ellipseBorderColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        private void shapeFillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().ellipseFillColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        private void chkPaint_Checked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Hand;
            chkDrawLines.IsChecked = false;
            EditMode.Instance().isPainting = true;
            EditMode.Instance().paintColor = "#" + fillColorPicker.SelectedColor.R.ToString("X2") + fillColorPicker.SelectedColor.G.ToString("X2") + fillColorPicker.SelectedColor.B.ToString("X2");      
        }

        private void chkPaint_Unchecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Arrow;
            EditMode.Instance().isPainting = false;
        }

        private void fillColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().paintColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");                 
        }
    }
}

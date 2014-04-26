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
using System.Threading.Tasks;
using Tennis.ApplicationGUI.VisualizationModes;

namespace Tennis.ApplicationGUI
{
    /// <summary>
    /// Interaction logic for Window GUI
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            selectedMode = VisualizationMode.Mode.FIRE;
            ApplicationController.Instance().designsReady_EventHandler += new EventHandler<TennisEventArgs>(this.onDesignsListRecieved);
            ApplicationController.Instance().designCreationStatusFailed_EventHandler += new EventHandler<TennisEventArgs>(this.onDesignCreationFailed);
            ApplicationController.Instance().designDataReady_EventHandler += new EventHandler<TennisEventArgs>(this.onSelectedDesignDataRecieved);
            ApplicationController.Instance().designDurationUpdated_EventHandler += new EventHandler<TennisEventArgs>(this.onDesignFinishedLoading);
        }

        #region "GUI Listeners"
        private void onMainWindowLoad(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Visible));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = true));
            ApplicationController.Instance().requestDesignsFromDataBase();
        }

        private void onBtnNewDesignClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!insertNameMessageView.IsVisible)
            {
                insertNameMessageView.Visibility = Visibility.Visible;
                txtNewName.Focus();
            }
        }

        private void onTxtNewNameKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        private void onRdbDrawFireIsChecked(object sender, RoutedEventArgs e)
        {
            selectedMode = VisualizationMode.Mode.FIRE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
                this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode));

        }

        private void onRdbDrawArcadeIsChecked(object sender, RoutedEventArgs e)
        {
            selectedMode = VisualizationMode.Mode.ARCADE;
            if (ApplicationController.Instance().getCurrentDesign() != null)
                this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode));

        }

        private void onBtnSaveDesignClick(object sender, System.Windows.RoutedEventArgs e)
        {
            beginViewModeSaving(true);
        }

        private void onBtnEditDesignClick(object sender, System.Windows.RoutedEventArgs e)
        {
            beginEditingMode();
        }

        private void onBtnCancelEditClick(object sender, System.Windows.RoutedEventArgs e)
        {
            beginViewModeSaving(false);
        }

        private void onlineThicknessSldValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lineThicknessLbl.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().newLinesThickness = (int)e.NewValue;
            }
            catch (NullReferenceException) { }
        }

        private void onSldBorderThicknessValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblBorderThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().setBorderThickness((int)e.NewValue);
            }
            catch (NullReferenceException) { }
        }

        private void onSldBaseLineThicknessValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblBaseLineThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().setBaseLineThickness((int)e.NewValue);
            }
            catch (NullReferenceException) { }
        }

        private void onBorderColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().setBorderColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));
        }

        private void onBaseLineColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().setBaseLineColor("#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2"));
        }

        private void onLineColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().newLinesColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        private void onChkDrawLinesIsChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Hand;
            chkPaint.IsChecked = false;
            EditMode.Instance().isDrawingLines = true;
            EditMode.Instance().newLinesColor = "#" + lineColorPicker.SelectedColor.R.ToString("X2") + lineColorPicker.SelectedColor.G.ToString("X2") + lineColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().newLinesThickness = (int)lineThicknessSld.Value;
        }

        private void onChkDrawLinesIsUnchecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Arrow;
            EditMode.Instance().isDrawingLines = false;
        }

        private void onBtnReportsClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!currentDesignReportsView.IsVisible)
            {
                currentDesignReportsView.Visibility = Visibility.Visible;
                currentDesignReportsView.Focus();
            }
            else
            {
                currentDesignReportsView.Visibility = Visibility.Hidden;
            }
        }

        private void onElementMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            insertNameMessageView.Visibility = Visibility.Hidden;
            txtNewName.Clear();
            currentDesignReportsView.Visibility = Visibility.Hidden;
        }

         private void onBtnDrawEllipseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EditMode.Instance().isDrawingEllipse = true;
            designerView.Cursor = Cursors.Hand;
            EditMode.Instance().ellipseBorderColor = "#" + shapeBorderColorPicker.SelectedColor.R.ToString("X2") + shapeBorderColorPicker.SelectedColor.G.ToString("X2") + shapeBorderColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().ellipseFillColor = "#" + shapeFillColorPicker.SelectedColor.R.ToString("X2") + shapeFillColorPicker.SelectedColor.G.ToString("X2") + shapeFillColorPicker.SelectedColor.B.ToString("X2");
            EditMode.Instance().ellipseRadius = (int)sldEllipseRadius.Value;
            EditMode.Instance().ellipseThickness = (int)sldEllipseThickness.Value;
        }

        private void onSldEllipseThicknessValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblEllipseThickness.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().ellipseThickness = ((int)e.NewValue);
            }
            catch (NullReferenceException) { } 
        }

        private void onSldEllipseRadiusValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lblEllipseRadius.Content = ((int)e.NewValue).ToString() + "px";
                EditMode.Instance().ellipseRadius = ((int)e.NewValue);
            }
            catch (NullReferenceException) { } 
        }

        private void onShapeBorderColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().ellipseBorderColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        private void onShapeFillColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().ellipseFillColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        private void onChkPaintIsChecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Hand;
            chkDrawLines.IsChecked = false;
            EditMode.Instance().isPainting = true;
            EditMode.Instance().paintColor = "#" + fillColorPicker.SelectedColor.R.ToString("X2") + fillColorPicker.SelectedColor.G.ToString("X2") + fillColorPicker.SelectedColor.B.ToString("X2");      
        }

        private void onChkPaintIsUnchecked(object sender, RoutedEventArgs e)
        {
            designerView.Cursor = Cursors.Arrow;
            EditMode.Instance().isPainting = false;
        }

        private void onFillColorPickerSelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            EditMode.Instance().paintColor = "#" + e.NewValue.R.ToString("X2") + e.NewValue.G.ToString("X2") + e.NewValue.B.ToString("X2");
        }

        #endregion

        #region "Application Event Listeners"

        private void onDesignCreationFailed(object sender, TennisEventArgs args)
        {
            if (!args.FinishedSuccessfully)
            {
                MessageBox.Show("Ha ocurrido un error al intentar crear el nuevo diseño.", "Error Inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void onDesignItemSelected(object sender, TennisEventArgs args)
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

        private void onDesignsListRecieved(object sender, TennisEventArgs args)
        {
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
            Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = false));
            if (args.FinishedSuccessfully)
            {
                foreach (string[] obj in args.DesignsList)
                {
                    Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton(obj[0], obj[1], obj[2], false, onDesignItemSelected))));
                }
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar cargar los diseños. Por favor verifique su conexión y vuelva a intentarlo.", "Error al Conectar", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
        }

        private void onSelectedDesignDataRecieved(object sender, TennisEventArgs args)
        {
            if (args.FinishedSuccessfully)
            {
                if (args.IsNewDesign)
                {
                    Dispatcher.BeginInvoke(new Action(() => desingsList.Children.Add(new DesignButton((string)args.DesignData[0], (string)args.DesignData[1],
                        (string)args.DesignData[2], true, onDesignItemSelected))));
                    Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                    Dispatcher.BeginInvoke(new Action(() => currentDesignReportsView.Visibility = Visibility.Hidden));
                    Dispatcher.BeginInvoke(new Action(() => btnNewDesign.IsEnabled = true));
                    Dispatcher.BeginInvoke(new Action(() => waitingProgress.Visibility = Visibility.Hidden));
                    Dispatcher.BeginInvoke(new Action(() => waitingProgress.IsIndeterminate = false));
                    Dispatcher.BeginInvoke(new Action(() => btnReports.IsEnabled = true));
                }
                else
                {
                    this.drawDesignUsingMode(VisualizationMode.createInstance(ApplicationController.Instance(), this.designerView, selectedMode));

                    Dispatcher.BeginInvoke(new Action(() => lblDesignName.Content = (string)args.DesignData[1]));
                    Dispatcher.BeginInvoke(new Action(() => btnEditDesign.IsEnabled = true));
                }
            }

            else
            {
                MessageBox.Show("Ha ocurrido un error al intentar cargar el diseño. Por favor verifique su conexión y vuelva a intentarlo.", "Error al Conectar", MessageBoxButton.OK, MessageBoxImage.Error);           
            }
        }

        private void onDesignFinishedLoading(object sender, TennisEventArgs args)
        {
            lblFireDuration.Content = ApplicationController.Instance().getCurrentDesignFireDuration();
            lblFireDate.Content = ApplicationController.Instance().getCurrentDesignFireDurationDate();
            lblArcadeDuration.Content = (ApplicationController.Instance().getCurrentDesignArcadeDuration());
            lblArcadeDate.Content = ApplicationController.Instance().getCurrentDesignArcadeDurationDate();

            designerView.root.Children.Clear();
            Image finishedDesign = new Image();
            finishedDesign.Source = args.DesignBitmap;
            designerView.AddShape(finishedDesign);

            btnNewDesign.IsEnabled = true;
            waitingProgress.Visibility = Visibility.Hidden;
            waitingProgress.IsIndeterminate = false;
            btnReports.IsEnabled = true;
        }

        private void drawDesignUsingMode(VisualizationMode pMode)
        {
            designerView.root.Children.Clear();
            pMode.initDrawing();
        }

        private void beginEditingMode()
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

        private void beginViewModeSaving(bool pSaving)
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

        #endregion

        private VisualizationMode.Mode selectedMode;  

    }
}

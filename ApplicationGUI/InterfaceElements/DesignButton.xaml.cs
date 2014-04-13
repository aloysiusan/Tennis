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
using Tennis.TEventArgs;

namespace Tennis.ApplicationGUI
{
	/// <summary>
	/// Lógica de interacción para DesignButton.xaml
	/// </summary>
	public partial class DesignButton : UserControl
	{
        public event EventHandler<TennisEventArgs> designButtonSelected_EventHandler;
        public event EventHandler<TennisEventArgs> designButtonPreviewSelected_EventHandler;

        private String _ID;
        private bool isSelected;
        private static List<DesignButton> instances = new List<DesignButton>();
        private static readonly SolidColorBrush THEME_BLUE = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x80, 0xFF));

        public DesignButton(String pID, String pName, String pCreationDate, bool pSelected, EventHandler<TennisEventArgs> OnDesignItemSelected)
		{
			this.InitializeComponent();
            _ID = pID;
            lblName.Content = pName;
            lblCreationDate.Content = pCreationDate;
            instances.Add(this);
            this.designButtonSelected_EventHandler += OnDesignItemSelected;
            if (pSelected) setSelected(true);
		}

        public static List<DesignButton> getInstances()
        {
            return instances;
        }

        public void setSelected(bool asNewDesign)
        {
            if (!this.isSelected) {
                foreach (DesignButton instance in instances)
                {
                    if (instance.Equals(this))
                    {
                        this.isSelected = true;
                        this.LayoutRoot.Background = THEME_BLUE;
                        this.lblName.Foreground = Brushes.White;
                        this.lblCreationDate.Foreground = Brushes.White;
                    }
                    else
                    {
                        instance.isSelected = false;
                        instance.LayoutRoot.Background = Brushes.Transparent;
                        instance.lblName.Foreground = THEME_BLUE;
                        instance.lblCreationDate.Foreground = Brushes.Gray;
                    }
                }            
            }

            EventHandler<TennisEventArgs> handler = designButtonSelected_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.IsNewDesign = asNewDesign;
                args.SelectedDesignID = this._ID;
                handler(this, args);
            }
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            ((DesignButton)sender).setSelected(false);
        }

        public String getName(){
            return lblName.Content.ToString();
        }

	}
}
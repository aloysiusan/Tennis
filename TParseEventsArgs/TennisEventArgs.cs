using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Tennis.TEventArgs
{
    public class TennisEventArgs : EventArgs
    {
        public enum Mode{
            FIRE, ARCADE
        }

        private String _ParseJSONData;
        private bool _FinishedSuccessfully;
        private bool _isNewDesign;
        private String _selectedDesignID;
        private object _ParseObjectData;
        private object[] _DesignData;
        private List<object> _DesignsList;
        private float _DrawDuration;
        private Mode _VisualizationMode;
        private WriteableBitmap _DesignBitmap;

        public String ParseJSONData
        {
            get { return _ParseJSONData; }
            set { _ParseJSONData = value; }
        }

        public object ParseObjectData
        {
            get { return _ParseObjectData; }
            set { _ParseObjectData = value; }
        }

        public object[] DesignData
        {
            get { return _DesignData; }
            set { _DesignData = value; }
        }

        public List<Object> DesignsList
        {
            get { return _DesignsList; }
            set { _DesignsList = value; }
        }

        public bool FinishedSuccessfully
        {
            get { return _FinishedSuccessfully; }
            set { _FinishedSuccessfully = value; }
        }

        public bool IsNewDesign
        {
            get { return _isNewDesign; }
            set { _isNewDesign = value; }
        }

        public string SelectedDesignID
        {
            get { return _selectedDesignID; }
            set { _selectedDesignID = value; }
        }

        public float DrawDuration
        {
            get { return _DrawDuration; }
            set { _DrawDuration = value; }
        }

        public WriteableBitmap DesignBitmap
        {
            get { return _DesignBitmap; }
            set { _DesignBitmap = value; }
        }

        public Mode VisualizationMode
        {
            get { return _VisualizationMode; }
            set { _VisualizationMode = value; }
        }
    }
}

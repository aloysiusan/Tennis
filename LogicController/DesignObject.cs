using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;

namespace Tennis.ApplicationLogic
{
    /// <summary>
    /// Design data stored in database.
    /// </summary>
    public class DesignObject
    {
        public DesignObject(string pID, string pName, string pUpdatedAt, TDesign pData, float pFireDuration, String pFireDate, float pArcadeDuration, String pArcadeDate)
        {
            _Id = pID;
            _Name = pName;
            _UpdatedAt = pUpdatedAt;
            _Data = pData;
            _FireDrawingDuration = pFireDuration;
            _FireBestDurationDate = pFireDate;
            _ArcadeDrawingDuration = pArcadeDuration;
            _ArcadeDrawingDurationDate = pArcadeDate;
        }

        public string ID
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string UpdatedAt
        {
            get { return _UpdatedAt; }
            set { _UpdatedAt = value; }
        }

        public TDesign DesignData
        {
            get { return _Data; }
            set { _Data = value; }
        }

        public float FireModeDrawingDuration
        {
            get { return _FireDrawingDuration; }
            set { _FireDrawingDuration = value; }
        }

        public string FireModeBestDurationDate
        {
            get { return _FireBestDurationDate; }
            set { _FireBestDurationDate = value; }
        }

        public float ArcadeModeDrawingDuration
        {
            get { return _ArcadeDrawingDuration; }
            set { _ArcadeDrawingDuration = value; }
        }

        public string ArcadeModeBestDurationDate
        {
            get { return _ArcadeDrawingDurationDate; }
            set { _ArcadeDrawingDurationDate = value; }
        }


        private string _Id;
        private string _Name;
        private string _UpdatedAt;
        private TDesign _Data;
        private float _FireDrawingDuration;
        private String _FireBestDurationDate;
        private float _ArcadeDrawingDuration;
        private String _ArcadeDrawingDurationDate;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    /// <summary>
    /// Logical indicator for design fill color.
    /// </summary>
    public class TFillIndicator : TPoint
    {
        public TFillIndicator(char pID, double pDesignerWidth, double pDesignerHeight, DefaultPosition pDefaultPositionX, DefaultPosition pDefaultPositionY) : base(pID, pDesignerWidth, pDesignerHeight, pDefaultPositionX, pDefaultPositionY)
        {
            _Id = pID;
            _XPosition = pDesignerWidth * (double)pDefaultPositionX / 10 - POSITION_OFFSET;
            _YPosition = pDesignerHeight * (double)pDefaultPositionY / 10 - POSITION_OFFSET;
            _RelativeXPosition = (double)pDefaultPositionX;
            _RelativeYPosition = (double)pDefaultPositionY;
        }

        public String OldFillColor
        {
            get { return _OldFillColor; }
            set { _OldFillColor = value; }
        }

        public String NewFillColor
        {
            get { return _NewFillColor; }
            set { _NewFillColor = value; }
        }

        /// <summary>
        /// Creates an identical clone of this instance.
        /// </summary>
        public new TFillIndicator Clone()
        {
            TFillIndicator thisClone = new TFillIndicator(this._Id, 0, 0, DefaultPosition.DefaultGeneric_XPosition, DefaultPosition.DefaultGeneric_YPosition);
            thisClone._XPosition = this._XPosition;
            thisClone._YPosition = this._YPosition;
            thisClone._RelativeXPosition = this._RelativeXPosition;
            thisClone._RelativeYPosition = this._RelativeYPosition;
            thisClone._NewFillColor = this._NewFillColor;
            thisClone._OldFillColor = this._OldFillColor;
            return thisClone;
        }

        private string _NewFillColor;
        private string _OldFillColor;
    }
}

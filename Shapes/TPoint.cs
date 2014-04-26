using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes{
    /// <summary>
    /// Logical coodinates Point for design.
    /// </summary>
    public class TPoint
    {
        /// <summary>
        /// Default relative position for design border points.
        /// </summary>
        public enum DefaultPosition
        {
            DefaultPointA_XPosition = 1,
            DefaultPointB_XPosition = 4,
            DefaultPointC_XPosition = 6,
            DefaultPointD_XPosition = 9,
            DefaultPointE_XPosition = 1,
            DefaultPointA_YPosition = 2,
            DefaultPointB_YPosition = 1,
            DefaultPointC_YPosition = 5,
            DefaultPointD_YPosition = 9,
            DefaultPointE_YPosition = 9,
            DefaultGeneric_XPosition = 0,
            DefaultGeneric_YPosition = 0
        }

        public TPoint(char pID, double pDesignerWidth, double pDesignerHeight, DefaultPosition pDefaultPositionX, DefaultPosition pDefaultPositionY)
        {
            _Id = pID;
            _XPosition = pDesignerWidth * (double)pDefaultPositionX/10 - POSITION_OFFSET;
            _YPosition = pDesignerHeight * (double)pDefaultPositionY/10 - POSITION_OFFSET;
            _RelativeXPosition = (double)pDefaultPositionX;
            _RelativeYPosition = (double)pDefaultPositionY;
        }

        public char ID
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public double XPosition
        {
            get { return _XPosition; }
            set { _XPosition = value; }
        }

        public double YPosition
        {
            get { return _YPosition; }
            set { _YPosition = value; }
        }

        public double RelativeXPosition
        {
            get { return _RelativeXPosition; }
            set { _RelativeXPosition = value; }
        }

        public double RelativeYPosition
        {
            get { return _RelativeYPosition; }
            set { _RelativeYPosition = value; }
        }

        /// <summary>
        /// Sets the current instance as a not default (border position) point.
        /// </summary>
        public void setPointAsCustom(double pDesignerWidth, double pDesignerHeight, double pXPosition, double pYPosition)
        {
            _XPosition = pXPosition - POSITION_OFFSET;
            _YPosition = pYPosition - POSITION_OFFSET;
            _RelativeXPosition = pXPosition * 10 / pDesignerWidth;
            _RelativeYPosition = pYPosition * 10 / pDesignerHeight;
        }

        /// <summary>
        /// Sets the relative X coordinate according the actual position.
        /// </summary>
        public void setRelativeXPosition(double pPosition,double relativeWidth){
            _XPosition = pPosition;
            _RelativeXPosition = 10 * (pPosition - POSITION_OFFSET) / relativeWidth;
        }

        /// <summary>
        /// Sets the relative Y coordinate according the actual position.
        /// </summary>
        public void setRelativeYPosition(double pPosition, double relativeHeight)
        {
            _YPosition = pPosition;
            _RelativeYPosition = 10 * (pPosition - POSITION_OFFSET) / relativeHeight;
        }

        /// <summary>
        /// Adjust point actual position according the relative value.
        /// </summary>
        public void adjustPosition(double pRelativeWidth, double pRelativeHeight)
        {
            _XPosition = pRelativeWidth * (double)_RelativeXPosition / 10 - POSITION_OFFSET;
            _YPosition = pRelativeHeight * (double)_RelativeYPosition / 10 - POSITION_OFFSET;            
        }

        /// <summary>
        /// Gets point ID.
        /// </summary>
        public char getID()
        {
            return _Id;
        }

        /// <summary>
        /// Creates an identical clone of this instance.
        /// </summary>
        public TPoint Clone()
        {
            TPoint thisClone = new TPoint(this._Id, 0, 0, DefaultPosition.DefaultGeneric_XPosition, DefaultPosition.DefaultGeneric_YPosition);
            thisClone._XPosition = this._XPosition;
            thisClone._YPosition = this._YPosition;
            thisClone._RelativeXPosition = this._RelativeXPosition;
            thisClone._RelativeYPosition = this._RelativeYPosition;
            return thisClone;
        }

        public static readonly double RADIUS = 4;
        public static readonly String DEFAULT_COLOR = "#FF0000";

        protected readonly int POSITION_OFFSET = 2;

        protected char _Id;
        protected double _XPosition = 0;
        protected double _YPosition = 0;
        protected double _RelativeXPosition = 0;
        protected double _RelativeYPosition = 0;
    }
}

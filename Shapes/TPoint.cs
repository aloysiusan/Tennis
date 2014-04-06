using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes{
    public class TPoint
    {
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
            DefaultPointE_YPosition = 9
        }

        public char id;
        public double XPosition = 0;
        public double YPosition = 0;
        public double globalXPositionPercent = 0;
        public double globalYPositionPercent = 0;
        public static readonly double RADIUS = 4;
        private readonly int POSITION_OFFSET = 2; /* RADIUS / 2 */
        public static readonly String COLOR = "#FF0000";

        public TPoint(char pID, double pDesignerWidth, double pDesignerHeight, DefaultPosition pDefaultPositionX, DefaultPosition pDefaultPositionY)
        {
            id = pID;
            XPosition = pDesignerWidth * (double)pDefaultPositionX/10 - POSITION_OFFSET;
            YPosition = pDesignerHeight * (double)pDefaultPositionY/10 - POSITION_OFFSET;
            globalXPositionPercent = (double)pDefaultPositionX;
            globalYPositionPercent = (double)pDefaultPositionY;

        }

        public double getXPosition(){
            return XPosition;
        }

        public double getYPosition()
        {
            return YPosition;
        }

        public void setXPositionRelative(double pPosition,double relativeWidth){
            XPosition = pPosition;
            globalXPositionPercent = 10 * (pPosition - POSITION_OFFSET) / relativeWidth;
        }

        public void setYPositionRelative(double pPosition, double relativeHeight)
        {
            YPosition = pPosition;
            globalYPositionPercent = 10 * (pPosition - POSITION_OFFSET) / relativeHeight;
        }

        public void adjustPosition(double pRelativeWidth, double pRelativeHeight)
        {
            XPosition = pRelativeWidth * (double)globalXPositionPercent / 10 - POSITION_OFFSET;
            YPosition = pRelativeHeight * (double)globalYPositionPercent / 10 - POSITION_OFFSET;            
        }
        public char getID()
        {
            return id;
        }
    }
}

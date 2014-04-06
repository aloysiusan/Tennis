using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Shapes;

namespace Tennis.Design
{
    /* Porcentaje de localización inicial X de los puntos en el Canvas:
     * A = 10%
     * B = 40%
     * C = 60%
     * D = 90%
     * E = 10%
     */

    /* Porcentaje de localización inicial Y de los puntos en el Canvas:
    * A = 20%
    * B = 10%
    * C = 50%
    * D = 90%
    * E = 90%
    */
    public class TDesign
    {
        public TPoint pointA;
        public TPoint pointB;
        public TPoint pointC;
        public TPoint pointD;
        public TPoint pointE;

        public TLine[] designLines;
        public TLine baseLine; //Suela
        public TArc[] designArcs;       
        

        public TDesign( double pContainerWidth, double pContainerHeight)
        {
            pointA = new TPoint('a', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointA_XPosition, TPoint.DefaultPosition.DefaultPointA_YPosition);
            pointB = new TPoint('b', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointB_XPosition, TPoint.DefaultPosition.DefaultPointB_YPosition);
            pointC = new TPoint('c', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointC_XPosition, TPoint.DefaultPosition.DefaultPointC_YPosition);
            pointD = new TPoint('d', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointD_XPosition, TPoint.DefaultPosition.DefaultPointD_YPosition);
            pointE = new TPoint('e', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointE_XPosition, TPoint.DefaultPosition.DefaultPointE_YPosition);

            designLines = new TLine[2];
            designArcs = new  TArc[2];

            designLines[0] = new TLine(pointB, pointC);
            designLines[1] = new TLine(pointC, pointD);
            
            designArcs[0] = new TArc(pointE, pointA, false);
            designArcs[1] = new TArc(pointA, pointB, false);

            baseLine = new TLine(pointE, pointD);
        }

        public TPoint getPointWithID(char pID)
        {
            switch (pID)
            {
                case 'a':
                    return pointA;
                case 'b':
                    return pointB;
                case 'c':
                    return pointC;
                case 'd':
                    return pointD;
                case 'e':
                    return pointE;
                default:
                    return null;
            }
        }

        /*public void createLine(TPoint pStartPoint, TPoint pEndPoint){
            TLine line = new TLine(pStartPoint, pEndPoint);
            designLines.Add(line);
        }

        public void createArc(TPoint pStartPoint, TPoint pEndPoint, bool pIsInverted) {
            TArc arc = new TArc(pStartPoint, pEndPoint, pIsInverted);
            designArcs.Add(arc);
        }
        */
        public void adjustPoints(double pRelativeWidth, double pRelativeHeight)
        {
            pointA.adjustPosition(pRelativeWidth, pRelativeHeight);
            pointB.adjustPosition(pRelativeWidth, pRelativeHeight);
            pointC.adjustPosition(pRelativeWidth, pRelativeHeight);
            pointD.adjustPosition(pRelativeWidth, pRelativeHeight);
            pointE.adjustPosition(pRelativeWidth, pRelativeHeight);

            designLines[0].startPoint = pointB;
            designLines[0].endPoint = pointC;
            designLines[1].startPoint = pointC;
            designLines[1].endPoint = pointD;

            designArcs[0].startPoint = pointE;
            designArcs[0].endPoint = pointA;
            designArcs[1].startPoint = pointA;
            designArcs[1].endPoint = pointB;

            baseLine.startPoint = pointE;
            baseLine.endPoint = pointD;
        }
    }
}

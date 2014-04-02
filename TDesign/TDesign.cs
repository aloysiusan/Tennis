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
        private TPoint pointA;
        private TPoint pointB;
        private TPoint pointC;
        private TPoint pointD;
        private TPoint pointE;

        private List<TLine> designLines;
        private List<TArc> designArcs;

        private String id;

        public TDesign(String pID, double pContainerWidth, double pContainerHeight)
        {
            pointA = new TPoint('a', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointA_XPosition, TPoint.DefaultPosition.DefaultPointA_YPosition);
            pointB = new TPoint('b', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointB_XPosition, TPoint.DefaultPosition.DefaultPointB_YPosition);
            pointC = new TPoint('c', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointC_XPosition, TPoint.DefaultPosition.DefaultPointC_YPosition);
            pointD = new TPoint('d', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointD_XPosition, TPoint.DefaultPosition.DefaultPointD_YPosition);
            pointE = new TPoint('e', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointE_XPosition, TPoint.DefaultPosition.DefaultPointE_YPosition);

            id = pID;

            designLines = new List<TLine>();
            designArcs = new  List<TArc>();

            this.createLine(pointB, pointC);
            this.createLine(pointC, pointD);
            this.createLine(pointE, pointD);
            this.createArc(pointE, pointA, false);
            this.createArc(pointA, pointB, true);
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

        public void createLine(TPoint pStartPoint, TPoint pEndPoint){
            TLine line = new TLine(pStartPoint, pEndPoint);
            designLines.Add(line);
        }

        public void createArc(TPoint pStartPoint, TPoint pEndPoint, bool pIsInverted) {
            TArc arc = new TArc(pStartPoint, pEndPoint, pIsInverted);
            designArcs.Add(arc);
        }

        public List<TLine> getDesignLines()
        {
            return designLines;
        }

        public List<TArc> getDesignArcs()
        {
            return designArcs;
        }
    }
}

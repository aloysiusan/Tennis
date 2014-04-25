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
        private TPoint _PointA;
        private TPoint _PointB;
        private TPoint _PointC;
        private TPoint _PointD;
        private TPoint _PointE;

        private TLine[] _BorderLines;
        private List<TLine> _CustomLines;

        private TLine _BaseLine;
        private TArc[] _BorderArcs;

        private List<TEllipse> _CustomEllipses;

        private List<TPoint> _FillIndicators;

        public TPoint PointA
        {
            get { return _PointA; }
            set { _PointA = value; }
        }

        public TPoint PointB
        {
            get { return _PointB; }
            set { _PointB = value; }
        }

        public TPoint PointC
        {
            get { return _PointC; }
            set { _PointC = value; }
        }

        public TPoint PointD
        {
            get { return _PointD; }
            set { _PointD = value; }
        }

        public TPoint PointE
        {
            get { return _PointE; }
            set { _PointE = value; }
        }

        public TLine[] BorderLines
        {
            get { return _BorderLines; }
            set { _BorderLines = value; }
        }

        public List<TLine> CustomLines
        {
            get { return _CustomLines; }
            set { _CustomLines = value; }
        }

        public TLine BaseLine
        {
            get { return _BaseLine; }
            set { _BaseLine = value; }
        }

        public TArc[] BorderArcs
        {
            get {return _BorderArcs;}
            set { _BorderArcs = value; }
        }

        public List<TEllipse> CustomEllipses
        {
            get { return _CustomEllipses; }
            set { _CustomEllipses = value; }
        }

        public List<TPoint> FillIndicators
        {
            get { return _FillIndicators; }
            set { _FillIndicators = value; }
        }

        public TDesign( double pContainerWidth, double pContainerHeight)
        {
            _PointA = new TPoint('a', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointA_XPosition, TPoint.DefaultPosition.DefaultPointA_YPosition);
            _PointB = new TPoint('b', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointB_XPosition, TPoint.DefaultPosition.DefaultPointB_YPosition);
            _PointC = new TPoint('c', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointC_XPosition, TPoint.DefaultPosition.DefaultPointC_YPosition);
            _PointD = new TPoint('d', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointD_XPosition, TPoint.DefaultPosition.DefaultPointD_YPosition);
            _PointE = new TPoint('e', pContainerWidth, pContainerHeight, TPoint.DefaultPosition.DefaultPointE_XPosition, TPoint.DefaultPosition.DefaultPointE_YPosition);

            _BorderLines = new TLine[2];
            _BorderArcs = new  TArc[2];
            _CustomLines = new List<TLine>();
            _CustomEllipses = new List<TEllipse>();
            _FillIndicators = new List<TPoint>();

            _BorderLines[0] = new TLine(_PointB, _PointC);
            _BorderLines[1] = new TLine(_PointC, _PointD);
            
            _BorderArcs[0] = new TArc(_PointE, _PointA, false);
            _BorderArcs[1] = new TArc(_PointA, _PointB, true);

            _BaseLine = new TLine(_PointE, _PointD);
        }

        public TPoint getPointWithID(char pID)
        {
            switch (pID)
            {
                case 'a':
                    return _PointA;
                case 'b':
                    return _PointB;
                case 'c':
                    return _PointC;
                case 'd':
                    return _PointD;
                case 'e':
                    return _PointE;
                default:
                    return null;
            }
        }

        public void setBorderThickness(int pValue)
        {
            _BorderLines[0].thickness = pValue;
            _BorderLines[1].thickness = pValue;
            _BorderArcs[0].thickness = pValue;
            _BorderArcs[1].thickness = pValue;
        }

        public void setBorderColor(String pColorValue)
        {
            _BorderLines[0].color = pColorValue;
            _BorderLines[1].color = pColorValue;
            _BorderArcs[0].color = pColorValue;
            _BorderArcs[1].color = pColorValue;
        }

        public void setBaseLineThickness(int pValue)
        {
            _BaseLine.thickness = pValue;
        }

        public void setBaseLineColor(String pColorValue)
        {
            _BaseLine.color = pColorValue;
        }

        public void adjustPoints(double pRelativeWidth, double pRelativeHeight)
        {
            _PointA.adjustPosition(pRelativeWidth, pRelativeHeight);
            _PointB.adjustPosition(pRelativeWidth, pRelativeHeight);
            _PointC.adjustPosition(pRelativeWidth, pRelativeHeight);
            _PointD.adjustPosition(pRelativeWidth, pRelativeHeight);
            _PointE.adjustPosition(pRelativeWidth, pRelativeHeight);

            _BorderLines[0].startPoint = _PointB;
            _BorderLines[0].endPoint = _PointC;
            _BorderLines[1].startPoint = _PointC;
            _BorderLines[1].endPoint = _PointD;

            _BorderArcs[0].startPoint = _PointE;
            _BorderArcs[0].endPoint = _PointA;
            _BorderArcs[1].startPoint = _PointA;
            _BorderArcs[1].endPoint = _PointB;

            _BaseLine.startPoint = _PointE;
            _BaseLine.endPoint = _PointD;
        }

        public TDesign Clone()
        {
            TDesign thisClone = new TDesign(0, 0);
            thisClone._PointA = this._PointA.Clone();
            thisClone._PointB = this._PointB.Clone();
            thisClone._PointC = this._PointC.Clone();
            thisClone._PointD = this._PointD.Clone();
            thisClone._PointE = this._PointE.Clone();

            thisClone._BorderLines[0] = this._BorderLines[0].Clone(thisClone._PointB, thisClone._PointC);
            thisClone._BorderLines[1] = this._BorderLines[1].Clone(thisClone._PointC, thisClone._PointD);

            thisClone._BorderArcs[0] = this._BorderArcs[0].Clone(thisClone._PointE, thisClone._PointA);
            thisClone._BorderArcs[1] = this._BorderArcs[1].Clone(thisClone._PointA, thisClone._PointB);

            thisClone._BaseLine = this._BaseLine.Clone(thisClone._PointE, thisClone._PointD);
            
            foreach(TLine line in this._CustomLines){
                thisClone._CustomLines.Add(line.Clone(line.startPoint.Clone(),line.endPoint.Clone()));
            }      
     
            foreach(TEllipse ellipse in this._CustomEllipses){
                thisClone._CustomEllipses.Add(ellipse.Clone(ellipse.radiusPoint));
            }

            foreach (TPoint fillPoint in this._FillIndicators)
            {
                thisClone._FillIndicators.Add(fillPoint.Clone());
            }

            return thisClone;
        }
    }
}

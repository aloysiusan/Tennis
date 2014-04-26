using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{

    /// <summary>
    /// Logical line shape.
    /// </summary>
    public class TLine
    {
        public TLine(TPoint pStartPoint, TPoint pEndPoint)
        {
            _StartPoint = pStartPoint;
            _EndPoint = pEndPoint;
        }

        public TPoint StartPoint
        {
            get { return _StartPoint; }
            set { _StartPoint = value; }
        }

        public TPoint EndPoint
        {
            get { return _EndPoint; }
            set { _EndPoint = value; }
        }

        public int Thickness
        {
            get { return _Thickness; }
            set { _Thickness = value; }
        }

        public String Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        /// <summary>
        /// Creates an identical clone of this instance.
        /// </summary>
        public TLine Clone(TPoint pStartPoint, TPoint pEndPoint)
        {
            TLine thisClone = new TLine(pStartPoint, pEndPoint);
            thisClone._Thickness = this._Thickness;
            thisClone._Color = this._Color;
            return thisClone;
        }

        private TPoint _StartPoint;
        private TPoint _EndPoint;
        private int _Thickness = 1;
        private String _Color = "#000000";
        public static readonly int POSITION_OFFSET = 2;
    }
}

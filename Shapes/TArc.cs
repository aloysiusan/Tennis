using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    /// <summary>
    /// Logial Arc shape for design.
    /// </summary>
    public class TArc
    {
        public TArc(TPoint pStartPoint, TPoint pEndPoint, bool pIsInverted)
        {
            _StartPoint = pStartPoint;
            _EndPoint = pEndPoint;
            _Inverted = pIsInverted;            
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

        public bool IsInverted
        {
            get { return _Inverted; }
            set { _Inverted = value; }
        }

        /// <summary>
        /// Creates an identical clone of this instance.
        /// </summary>
        public TArc Clone(TPoint startPoint, TPoint endPoint)
        {
            TArc thisClone = new TArc(startPoint, endPoint, _Inverted);
            thisClone._Thickness = this._Thickness;
            thisClone._Color = this._Color;
            return thisClone;
        }

        public TPoint _StartPoint;
        public TPoint _EndPoint;
        public int _Thickness = 1;
        public bool _Inverted;
        public String _Color = "#000000";
        public static readonly int POSITION_OFFSET = 2;
    }
}

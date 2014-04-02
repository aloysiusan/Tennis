using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    class TArc
    {
        public double angle = 0;
        public TPoint startPoint;
        public TPoint endPoint;
        public int thickness = 1;

        public TArc(TPoint pStartPoint, TPoint pEndPoint, double pAngle)
        {
            startPoint = pStartPoint;
            endPoint = pEndPoint;
            angle = pAngle;
        }
    }
}

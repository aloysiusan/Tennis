using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    public class TLine : TShape
    {
        public TPoint startPoint;
        public TPoint endPoint;
        public int thickness = 1;

        public TLine(TPoint pStartPoint, TPoint pEndPoint)
        {
            startPoint = pStartPoint;
            endPoint = pEndPoint;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    public class TLine
    {
        public TPoint startPoint;
        public TPoint endPoint;
        public int thickness = 1;
        public String color = "#000000";
        public static readonly int POSITION_OFFSET = 2; /* POINT RADIUS / 2 */

        public TLine(TPoint pStartPoint, TPoint pEndPoint)
        {
            startPoint = pStartPoint;
            endPoint = pEndPoint;
        }

        public TLine Clone(TPoint startPoint, TPoint endPoint)
        {
            TLine thisClone = new TLine(startPoint, endPoint);
            thisClone.thickness = this.thickness;
            thisClone.color = this.color;
            return thisClone;
        }
    }
}

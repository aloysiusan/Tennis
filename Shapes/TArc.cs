using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    public class TArc
    {
        public TPoint startPoint;
        public TPoint endPoint;
        public int thickness = 1;
        public bool inverted;
        public String color = "#000000";
        public static readonly int POSITION_OFFSET = 2; /* POINT RADIUS / 2 */

        public TArc(TPoint pStartPoint, TPoint pEndPoint, bool pIsInverted)
        {
            startPoint = pStartPoint;
            endPoint = pEndPoint;
            inverted = pIsInverted;            
        }

        public TArc Clone(TPoint startPoint, TPoint endPoint)
        {
            TArc thisClone = new TArc(startPoint, endPoint, inverted);
            thisClone.thickness = this.thickness;
            thisClone.color = this.color;
            return thisClone;
        }
    }
}

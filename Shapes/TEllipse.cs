using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    public class TEllipse
    {
        public TPoint radiusPoint;
        public int thickness = 1;
        public String fillColor = "#000000";
        public String borderColor = "#000000";
        public int radius = 1;

        public TEllipse(TPoint pRadiusPoint)
        {
            radiusPoint = pRadiusPoint;
        }

        public TEllipse Clone(TPoint pRadiusPoint)
        {
            TEllipse thisClone = new TEllipse(pRadiusPoint);
            thisClone.thickness = this.thickness;
            thisClone.fillColor = this.fillColor;
            thisClone.borderColor = this.borderColor;
            thisClone.radius = this.radius;
            return thisClone;
        }
    }
}

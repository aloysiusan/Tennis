using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.Shapes
{
    /// <summary>
    /// Logial Ellipse shape for design.
    /// </summary>
    public class TEllipse
    {
        public TEllipse(TPoint pRadiusPoint)
        {
            _RadiusPoint = pRadiusPoint;
        }

        public TPoint RadiusPoint
        {
            get { return _RadiusPoint; }
            set { _RadiusPoint = value; }
        }

        public int Thickness
        {
            get { return _Thickness; }
            set { _Thickness = value; }
        }

        public String BorderColor
        {
            get { return _BorderColor; }
            set { _BorderColor = value; }
        }

        public String FillColor
        {
            get { return _FillColor; }
            set { _FillColor = value; }
        }

        public int Radius
        {
            get { return _Radius; }
            set { _Radius = value; }
        }

        /// <summary>
        /// Creates an identical clone of this instance.
        /// </summary>
        public TEllipse Clone(TPoint pRadiusPoint)
        {
            TEllipse thisClone = new TEllipse(pRadiusPoint);
            thisClone._Thickness = this._Thickness;
            thisClone._FillColor = this._FillColor;
            thisClone._BorderColor = this._BorderColor;
            thisClone._Radius = this._Radius;
            return thisClone;
        }

        private TPoint _RadiusPoint;
        private int _Thickness = 1;
        private String _FillColor = "#000000";
        private String _BorderColor = "#000000";
        private int _Radius = 1;
    }
}

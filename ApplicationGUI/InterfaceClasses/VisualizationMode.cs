﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;
using Tennis.Shapes;

namespace Tennis.ApplicationGUI
{
    public abstract class VisualizationMode
    {

        public abstract void beginDrawingDesign();
        protected abstract void drawBorderLines(TLine[] pLines);
        protected abstract void drawBaseLine(TLine pLine);
        protected abstract void drawBorderArcs(TArc[] pArcs);

    }
}

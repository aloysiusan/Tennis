using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;
using Tennis.Shapes;

namespace Tennis.ApplicationGUI
{
    public class ArcadeVisualizationMode : VisualizationMode
    {
        private TDesign currentDesign;
        private Designer mainDesigner;

        private static ArcadeVisualizationMode _currentInstance;

        public static ArcadeVisualizationMode Instance()
        {
            return _currentInstance;
        }

        public static ArcadeVisualizationMode createSingleInstance(TDesign pDesign, Designer pSender)
        {
            destroyCurrentInstance();
            _currentInstance = new ArcadeVisualizationMode(pDesign, pSender);
            return _currentInstance;
        }

        public static void destroyCurrentInstance()
        {
            _currentInstance = null;
        }

        private ArcadeVisualizationMode(TDesign pDesign, Designer pSender)
        {
            currentDesign = pDesign;
            mainDesigner = pSender;
        }

        public override void beginDrawingDesign() { }
        protected override void drawBorderLines(TLine[] pLines) { }
        protected override void drawBaseLine(TLine pLine) { }
        protected override void drawBorderArcs(TArc[] pArcs) { }
    }
}

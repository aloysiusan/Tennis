using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;
using Tennis.Shapes;
using Tennis.ApplicationLogic;
using System.Windows.Threading;

namespace Tennis.ApplicationGUI.VisualizationModes
{
    public abstract class VisualizationMode
    {
        public enum Mode
        {
            FIRE, ARCADE, EDIT
        }

        public static VisualizationMode createInstance(ApplicationController pMainController, Designer pDesigner, Mode pSelectedMode){
            if (pSelectedMode == Mode.FIRE)
            {
                pMainController.getCurrentDesign().adjustPointsForRelativePosition(pDesigner.ActualWidth,pDesigner.ActualHeight);
                instance = FireMode.createInstance(pMainController.getCurrentDesign(), pDesigner);
                FireMode.Instance().finishDrawingDesign_EventHandler += pMainController.OnDesignFinishedDrawing;
            }
            else if (pSelectedMode == Mode.ARCADE)
            {
                pMainController.getCurrentDesign().adjustPointsForRelativePosition(pDesigner.ActualWidth, pDesigner.ActualHeight);
                instance = ArcadeMode.createNewInstance(pMainController.getCurrentDesign(), pDesigner);
                ArcadeMode.getInstance().finishDrawingDesign_EventHandler += pMainController.OnDesignFinishedDrawing;
            }
            else
            {
                pMainController.getCurrentDesign().adjustPointsForRelativePosition(pDesigner.ActualWidth, pDesigner.ActualHeight);
                instance = EditMode.createInstance(pMainController.getTmpDesign(), pDesigner);
            }

            return instance;
        }

        public abstract void initDrawing();

        private static VisualizationMode instance;
    }
}

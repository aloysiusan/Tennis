using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;
using Tennis.Shapes;
using Tennis.ApplicationLogic;
using System.Windows.Threading;

namespace Tennis.ApplicationGUI
{
    public abstract class VisualizationMode
    {
        private static VisualizationMode instance;
        public enum Mode
        {
            FIRE, ARCADE, EDIT
        }

        public static VisualizationMode createInstance(ApplicationController mainController, Designer pDesigner, Mode pSelectedMode){
            if (pSelectedMode == Mode.FIRE)
            {
                mainController.getCurrentDesign().adjustPoints(pDesigner.ActualWidth,pDesigner.ActualHeight);
                instance = FireMode.createInstance(mainController.getCurrentDesign(), pDesigner);
                FireMode.Instance().finishDrawingDesign_EventHandler += mainController.OnDesignFinishedDrawing;
            }
            else if (pSelectedMode == Mode.ARCADE)
            {
                mainController.getCurrentDesign().adjustPoints(pDesigner.ActualWidth, pDesigner.ActualHeight);
                instance = ArcadeMode.createInstance(mainController.getCurrentDesign(), pDesigner);
                ArcadeMode.Instance().finishDrawingDesign_EventHandler += mainController.OnDesignFinishedDrawing;
            }
            else
            {
                mainController.getCurrentDesign().adjustPoints(pDesigner.ActualWidth, pDesigner.ActualHeight);
                instance = EditMode.createInstance(mainController.getTmpDesign(), pDesigner);
            }

            return instance;
        }

        public abstract void initDrawing();
    }
}

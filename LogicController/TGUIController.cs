using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.LogicController
{
    public class TGUIController
    {
        private static TGUIController instance;

        private TGUIController() { }

        public static TGUIController Instance()
        {
            if (instance == null)
            {
                instance = new TGUIController();
            }
            return instance;
        }

        public void paintDesignWithID(int pID)
        {

        }

        public List<object> allDesignsList()
        {
            return AppMainController.Instance().getDesignsFromDataBase();
        }
    }
}

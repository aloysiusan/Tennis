using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.LogicController
{
    public class AppMainController
    {
        private static AppMainController instance;

        private AppMainController() { }

        public static AppMainController Instance()
        {
            if (instance == null)
            {
                instance = new AppMainController();
            }
            return instance;
        }

        public List<object> getDesignsFromDataBase()
        {
            return DataController.Instance().requestAllDesignsMainData();
        }
    }
}

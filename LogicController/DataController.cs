using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.ParseHandler;

namespace Tennis.LogicController
{
    public class DataController
    {
        private static DataController instance;

        private DataController() { }

        public static DataController Instance()
        {
            if (instance == null)
            {
                instance = new DataController();
            }
            return instance;
        }

        public List<object> requestAllDesignsMainData()
        {            
            return TParse.Instance().fetchAllDesignsMainInfo();
        }
    }
}

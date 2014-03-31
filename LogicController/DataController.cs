using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;
using Tennis.ParseHandler;
namespace Tennis.Controllers
{
    public class DataController
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationResponseRecieved_EventHandler;
        //*====================================*/

        private static DataController instance;

        private DataController() {
            TParse.Instance().designsDataFinishedDownloading_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignsMainDataRecieved);
            TParse.Instance().designCreationFinished_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFinished);
        }

        public static DataController Instance()
        {
            if (instance == null)
            {
                instance = new DataController();
            }
            return instance;
        }

        public void requestAllDesignsMainData()
        {
            TParse.Instance().fetchAllDesignsMainInfo();
        }

        public void OnDesignsMainDataRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designsFinishedDownloading_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void createNewDesign(String pName)
        {
            TParse.Instance().sendDataForNewDesign(pName, "");
        }

        public void OnDesignCreationFinished(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designCreationResponseRecieved_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}

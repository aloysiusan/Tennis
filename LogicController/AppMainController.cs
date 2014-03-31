using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;

namespace Tennis.Controllers
{
    public class AppMainController
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsReady_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationStatusReady_EventHandler;

        private static AppMainController instance;

        private AppMainController() {
            DataController.Instance().designsFinishedDownloading_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignsRecieved);
            DataController.Instance().designCreationResponseRecieved_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationResponseRecieved);
        }

        public static AppMainController Instance()
        {
            if (instance == null)
            {
                instance = new AppMainController();
            }
            return instance;
        }

        public void requestDesignsFromDataBase()
        {
            DataController.Instance().requestAllDesignsMainData();
        }

        public void OnDesignsRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designsReady_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void requestNewDesignCreation(String pName)
        {
            DataController.Instance().createNewDesign(pName);
        }

        public void OnDesignCreationResponseRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designCreationStatusReady_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;
using Tennis.Design;
using Tennis.Parse.Rows;

namespace Tennis.Controllers
{   

    public class AppMainController
    {
        public enum VisualizationMode
        {
            Arcade, 
            Fire, 
            Edit
        }

        //Event Handlers
        public event EventHandler<TennisEventArgs> designsReady_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationStatusFailed_EventHandler;
        public event EventHandler<TennisEventArgs> designDataReady_EventHandler;

        private static AppMainController instance;
        public VisualizationMode selectedMode;
        private object[] currentDesign;

        private AppMainController() {
            DataController.Instance().designsFinishedDownloading_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignsRecieved);
            DataController.Instance().designCreationResponseRecieved_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationResponseRecieved);
            DataController.Instance().designLoadResponseRecieved_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignDataRecieved);
        }

        public static AppMainController Instance()
        {
            if (instance == null)
            {
                instance = new AppMainController();
            }
            return instance;
        }

        public void setCurrentDesign(object[] pDesignData)
        {
            currentDesign = pDesignData;            
        }

        public TDesign getCurrentDesign()
        {            
            return (TDesign)currentDesign[3];
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
                ParseRow[] designsList = (ParseRow[])args.ParseObjectData;
                List<Object> formatedList = new List<object>();
                for (int i = 0; i < designsList.Length;i++)
                {
                    formatedList.Add(new string[] { designsList[i].id, designsList[i].name, designsList[i].createdAt});
                }
                args.DesignsList = formatedList;
                handler(this, args);
            }
        }

        public void requestNewDesignCreation(String pName)
        {
            DataController.Instance().createNewDesign(pName);
        }

        public void OnDesignCreationResponseRecieved(object sender, TennisEventArgs args)
        {            
            EventHandler<TennisEventArgs> handler = designCreationStatusFailed_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void requestDesignForID(String pID, bool pIsNew)
        {
            DataController.Instance().requestDesignDataForID(pID, pIsNew);
        }

        public void OnDesignDataRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designDataReady_EventHandler;
            if (handler != null)
            {                
                ParseRow currentDesignRow = (ParseRow)args.ParseObjectData;
                args.DesignData = new object[4] { currentDesignRow.id, currentDesignRow.name, currentDesignRow.createdAt, currentDesignRow.data };
                currentDesign = args.DesignData;                
                handler(this, args);
            }
        }
    }
}

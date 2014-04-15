using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;
using Tennis.Parse.Controller;
using Tennis.Parse.Rows;
using Newtonsoft.Json;

namespace Tennis.ApplicationLogic
{
    public class ParseDataController
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationResponseRecieved_EventHandler;
        public event EventHandler<TennisEventArgs> designLoadResponseRecieved_EventHandler;
        //*====================================*/

        private static ParseDataController instance;

        private ParseDataController() {
            ParseDataAccess.Instance().designsDataFinishedDownloading_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignsMainDataRecieved);
            ParseDataAccess.Instance().designCreationFailed_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFailed);
            ParseDataAccess.Instance().designLoadFinished_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignLoadFinished);
        }

        public static ParseDataController Instance()
        {
            if (instance == null)
            {
                instance = new ParseDataController();
            }
            return instance;
        }

        public void requestAllDesignsMainData()
        {
            ParseDataAccess.Instance().fetchAllDesignsMainInfo();
        }

        public void OnDesignsMainDataRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designsFinishedDownloading_EventHandler;
            if (handler != null)
            {
                args.ParseObjectData = JsonConvert.DeserializeObject<ParseRow[]>(args.ParseJSONData);
                handler(this, args);
            }
        }

        public void createNewDesign(String pName)
        {
            String JSONDefualtData = "{\"pointA\":{\"id\":\"a\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":2.0},\"pointB\":{\"id\":\"b\",\"globalXPositionPercent\":4.0,\"globalYPositionPercent\":1.0},\"pointC\":{\"id\":\"c\",\"globalXPositionPercent\":6.0,\"globalYPositionPercent\":5.0},\"pointD\":{\"id\":\"d\",\"globalXPositionPercent\":9.0,\"globalYPositionPercent\":9.0},\"pointE\":{\"id\":\"e\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":9.0},\"designLines\":[{\"startPoint\":{\"id\":\"b\",\"globalXPositionPercent\":4.0,\"globalYPositionPercent\":1.0},\"endPoint\":{\"id\":\"c\",\"globalXPositionPercent\":6.0,\"globalYPositionPercent\":5.0},\"thickness\":1,\"color\":\"#000000\"},{\"startPoint\":{\"id\":\"c\",\"globalXPositionPercent\":6.0,\"globalYPositionPercent\":5.0},\"endPoint\":{\"id\":\"d\",\"globalXPositionPercent\":9.0,\"globalYPositionPercent\":9.0},\"thickness\":1,\"color\":\"#000000\"}],\"baseLine\":{\"startPoint\":{\"id\":\"e\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":9.0},\"endPoint\":{\"id\":\"d\",\"globalXPositionPercent\":9.0,\"globalYPositionPercent\":9.0},\"thickness\":1,\"color\":\"#000000\"},\"designArcs\":[{\"angle\":0.0,\"startPoint\":{\"id\":\"e\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":9.0},\"endPoint\":{\"id\":\"a\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":2.0},\"thickness\":1,\"inverted\":false,\"color\":\"#000000\"},{\"angle\":0.0,\"startPoint\":{\"id\":\"a\",\"globalXPositionPercent\":1.0,\"globalYPositionPercent\":2.0},\"endPoint\":{\"id\":\"b\",\"globalXPositionPercent\":4.0,\"globalYPositionPercent\":1.0},\"thickness\":1,\"inverted\":true,\"color\":\"#000000\"}]}";
            ParseDataAccess.Instance().sendDataForNewDesign(pName, JSONDefualtData);
        }

        public void OnDesignCreationFailed(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designCreationResponseRecieved_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void requestDesignDataForID(String pID, bool pIsNew)
        {
            ParseDataAccess.Instance().fetchDesignDataForID(pID, pIsNew);
        }

        public void OnDesignLoadFinished(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designLoadResponseRecieved_EventHandler;
            if (handler != null)
            {               
                args.ParseObjectData = JsonConvert.DeserializeObject<ParseRow>(args.ParseJSONData);
                handler(this, args);
            }
        }

        public void prepareDesignForSaving(object pDesign, String pDesignId)
        {
            String designJson = JsonConvert.SerializeObject(pDesign);
            ParseDataAccess.Instance().saveDesign(designJson, pDesignId);
        }

        public void updateDesignDurationOnFire(String pID, double pDuration, DateTime pDate)
        {
            ParseDataAccess.Instance().updateDesignDuration(pID, pDuration, pDate, "fire");
        }

        public void updateDesignDurationOnArcade(String pID, double pDuration, DateTime pDate)
        {
            ParseDataAccess.Instance().updateDesignDuration(pID, pDuration, pDate, "arcade");
        }
    }
}

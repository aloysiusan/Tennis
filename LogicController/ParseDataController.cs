using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;
using Tennis.Parse.Controller;
using Newtonsoft.Json;

namespace Tennis.ApplicationLogic
{
    /// <summary>
    /// Controls recieved data from Parse Cloud service.
    /// </summary>
    public class ParseDataController
    {
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
            ParseDataAccess.Instance().requestDesignsList();
        }

        public void OnDesignsMainDataRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designsFinishedDownloading_EventHandler;
            if (handler != null)
            {
                args.ParseObjectData = JsonConvert.DeserializeObject<DesignObject[]>(args.ParseJSONData);
                handler(this, args);
            }
        }

        public void createNewDesign(String pName)
        {
            String JSONDefaultData = "{\"PointA\":{\"ID\":\"a\",\"RelativeXPosition\":1.0,\"RelativeYPosition\":2.0}," + 
                "\"PointB\":{\"ID\":\"b\",\"RelativeXPosition\":4.0,\"RelativeYPosition\":1.0},\"PointC\":{\"ID\":\"c\"," + 
                "\"RelativeXPosition\":6.0,\"RelativeYPosition\":5.0},\"PointD\":{\"ID\":\"d\",\"RelativeXPosition\":9.0," + 
                "\"RelativeYPosition\":9.0},\"PointE\":{\"ID\":\"e\",\"RelativeXPosition\":1.0,\"RelativeYPosition\":9.0}," + 
                "\"designLines\":[{\"StartPoint\":{\"ID\":\"b\",\"RelativeXPosition\":4.0,\"RelativeYPosition\":1.0}," + 
                "\"EndPoint\":{\"ID\":\"c\",\"RelativeXPosition\":6.0,\"RelativeYPosition\":5.0},\"Thickness\":1," + 
                "\"Color\":\"#000000\"},{\"StartPoint\":{\"ID\":\"c\",\"RelativeXPosition\":6.0,\"RelativeYPosition\":5.0}," + 
                "\"EndPoint\":{\"ID\":\"d\",\"RelativeXPosition\":9.0,\"RelativeYPosition\":9.0},\"Thickness\":1," + 
                "\"Color\":\"#000000\"}],\"BaseLine\":{\"StartPoint\":{\"ID\":\"e\",\"RelativeXPosition\":1.0," + 
                "\"RelativeYPosition\":9.0},\"EndPoint\":{\"ID\":\"d\",\"RelativeXPosition\":9.0,\"RelativeYPosition\":9.0}," + 
                "\"Thickness\":1,\"Color\":\"#000000\"},\"designArcs\":[{\"angle\":0.0,\"StartPoint\":{\"ID\":\"e\"," + 
                "\"RelativeXPosition\":1.0,\"RelativeYPosition\":9.0},\"EndPoint\":{\"ID\":\"a\",\"RelativeXPosition\":1.0," + 
                "\"RelativeYPosition\":2.0},\"Thickness\":1,\"IsInverted\":false,\"Color\":\"#000000\"},{\"angle\":0.0," + 
                "\"StartPoint\":{\"ID\":\"a\",\"RelativeXPosition\":1.0,\"RelativeYPosition\":2.0},\"EndPoint\":{\"ID\":\"b\"," + 
                "\"RelativeXPosition\":4.0,\"RelativeYPosition\":1.0},\"Thickness\":1,\"IsInverted\":true,\"Color\":\"#000000\"}]}";
            ParseDataAccess.Instance().sendDataForNewDesign(pName, JSONDefaultData);
        }

        public void OnDesignCreationFailed(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designCreationFailedResponseRecieved_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void requestDesignDataForID(String pID, bool pIsNew)
        {
            ParseDataAccess.Instance().requestDesignDataForID(pID, pIsNew);
        }

        public void OnDesignLoadFinished(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designLoadResponseRecieved_EventHandler;
            if (handler != null)
            {               
                args.ParseObjectData = JsonConvert.DeserializeObject<DesignObject>(args.ParseJSONData);
                handler(this, args);
            }
        }

        public void prepareDesignForSaving(object pDesign, String pDesignId)
        {
            String designJson = JsonConvert.SerializeObject(pDesign);
            ParseDataAccess.Instance().updateDesignData(designJson, pDesignId);
        }

        public void updateDesignDurationOnFire(String pID, double pDuration, DateTime pDate)
        {
            ParseDataAccess.Instance().updateDesignDuration(pID, pDuration, pDate, "fire");
        }

        public void updateDesignDurationOnArcade(String pID, double pDuration, DateTime pDate)
        {
            ParseDataAccess.Instance().updateDesignDuration(pID, pDuration, pDate, "arcade");
        }

        private static ParseDataController instance;

        //Event Handlers
        public event EventHandler<TennisEventArgs> designsFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationFailedResponseRecieved_EventHandler;
        public event EventHandler<TennisEventArgs> designLoadResponseRecieved_EventHandler;
        //*====================================*/
    }
}

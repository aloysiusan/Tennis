﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.TEventArgs;
using Tennis.Design;

namespace Tennis.ApplicationLogic
{   

    public class ApplicationController
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsReady_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationStatusFailed_EventHandler;
        public event EventHandler<TennisEventArgs> designDataReady_EventHandler;
        public event EventHandler<TennisEventArgs> designDurationUpdated_EventHandler;

        private static ApplicationController instance;        
        private object[] currentDesignData;
        private TDesign tmpDesign;

        private ApplicationController() {
            ParseDataController.Instance().designsFinishedDownloading_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignsRecieved);
            ParseDataController.Instance().designCreationFailedResponseRecieved_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignCreationFailedResponseRecieved);
            ParseDataController.Instance().designLoadResponseRecieved_EventHandler += new EventHandler<TennisEventArgs>(this.OnDesignDataRecieved);
        }

        public static ApplicationController Instance()
        {
            if (instance == null)
            {
                instance = new ApplicationController();
            }
            return instance;
        }

        public TDesign getTmpDesign()
        {
            tmpDesign = ((TDesign)currentDesignData[3]).Clone();
            return tmpDesign;
        }

        public TDesign getCurrentDesign()
        {
            return (currentDesignData != null) ? (TDesign)currentDesignData[3] : null;
        }

        public String getCurrentDesignFireDuration()
        {
            return ((float)currentDesignData[4] == 0) ? "-" : ((float)currentDesignData[4]).ToString() + "ms";
        }

        public String getCurrentDesignArcadeDuration()
        {
            return ((float)currentDesignData[6] == 0) ? "-" : ((float)currentDesignData[6]).ToString() + "ms";
        }

        public String getCurrentDesignFireDurationDate()
        {
            return "Fecha: " + currentDesignData[5].ToString();
        }

        public String getCurrentDesignArcadeDurationDate()
        {
            return "Fecha: " + currentDesignData[7].ToString();
        }

        public void requestDesignsFromDataBase()
        {
            ParseDataController.Instance().requestAllDesignsMainData();
        }

        public void OnDesignsRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designsReady_EventHandler;
            if (handler != null)
            {
                if (args.FinishedSuccessfully)
                {
                    DesignObject[] designsList = (DesignObject[])args.ParseObjectData;
                    List<Object> formatedList = new List<object>();
                    for (int i = 0; i < designsList.Length; i++)
                    {
                        formatedList.Add(new string[] { designsList[i].ID, designsList[i].Name, designsList[i].UpdatedAt });
                    }
                    args.DesignsList = formatedList;
                }
                handler(this, args);
            }
        }

        public void requestNewDesignCreation(String pName)
        {
            ParseDataController.Instance().createNewDesign(pName);
        }

        public void OnDesignCreationFailedResponseRecieved(object sender, TennisEventArgs args)
        {            
            EventHandler<TennisEventArgs> handler = designCreationStatusFailed_EventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public void requestDesignForID(String pID, bool pIsNew)
        {
            ParseDataController.Instance().requestDesignDataForID(pID, pIsNew);
        }

        public void OnDesignDataRecieved(object sender, TennisEventArgs args)
        {
            EventHandler<TennisEventArgs> handler = designDataReady_EventHandler;
            if (handler != null)
            {                
                DesignObject currentDesignObject = (DesignObject)args.ParseObjectData;
                args.DesignData = new object[8] { currentDesignObject.ID, currentDesignObject.Name, currentDesignObject.UpdatedAt, currentDesignObject.DesignData, 
                    currentDesignObject.FireModeDrawingDuration, currentDesignObject.FireModeDrawingDuration, currentDesignObject.ArcadeModeDrawingDuration, currentDesignObject.ArcadeModeBestDurationDate};
                currentDesignData = args.DesignData;
                handler(this, args);           
            }
        }

        public void saveCurrentDesign()
        {
            currentDesignData[3] = tmpDesign;
            ParseDataController.Instance().prepareDesignForSaving(currentDesignData[3],(string)currentDesignData[0]);
        }

        public void OnDesignFinishedDrawing(object sender, TennisEventArgs args)
        {
            if (args.VisualizationMode == TennisEventArgs.Mode.FIRE)
            {
                if (args.DrawDuration < (float)currentDesignData[4] || (float)currentDesignData[4] == 0)
                {
                    currentDesignData[4] = args.DrawDuration;
                    currentDesignData[5] = DateTime.Now;
                    ParseDataController.Instance().updateDesignDurationOnFire((string)currentDesignData[0], args.DrawDuration, DateTime.Now);
                }
            }
            else
            {
                if (args.DrawDuration < (float)currentDesignData[6] || (float)currentDesignData[6] == 0)
                {
                    currentDesignData[6] = args.DrawDuration;
                    currentDesignData[7] = DateTime.Now;
                    ParseDataController.Instance().updateDesignDurationOnArcade((string)currentDesignData[0], args.DrawDuration, DateTime.Now);
                }
            }

            EventHandler<TennisEventArgs> handler = designDurationUpdated_EventHandler;
            if (handler != null)
            {                
                handler(this, args);
            }
        }
    }
}

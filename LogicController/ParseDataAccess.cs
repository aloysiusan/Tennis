using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.Diagnostics;
using Tennis.TEventArgs;

namespace Tennis.Parse.Controller
{
    /// <summary>
    /// Data access controller for Parse Cloud service database.
    /// </summary>
    public class ParseDataAccess
    {
        private ParseDataAccess() { }

        public static ParseDataAccess Instance()
        {
            if (instance == null)
            {
                instance = new ParseDataAccess();
            }
            return instance;
        }

        public async void requestDesignsList()
        {
            String jsonResponse = "[";
            var query = ParseObject.GetQuery("Designs");
            await query.OrderBy("name").FindAsync().ContinueWith(t =>
            {
                bool success = true;
                try
                {
                    IEnumerable<ParseObject> results = t.Result;
                    foreach (var obj in results){
                        jsonResponse += "{\"ID\":\"" + (string)obj.ObjectId + "\", \"Name\":\"" + obj.Get<string>("name") + "\",\"UpdatedAt\":\"" + ((DateTime)obj.UpdatedAt).ToLocalTime().ToString() + "\"},";                        
                    }
                    jsonResponse += "]";
                 }
                catch (AggregateException) { success = false; }
                finally
                {
                    EventHandler<TennisEventArgs> handler = designsDataFinishedDownloading_EventHandler;
                    if (handler != null)
                    {
                        TennisEventArgs args = new TennisEventArgs();
                        args.ParseJSONData = jsonResponse;
                        args.FinishedSuccessfully = success;
                        handler(this, args);
                    }
                }
            });
            
        }

        public async void sendDataForNewDesign(String pName, String pData)
        {
            ParseObject newDesign = new ParseObject("Designs");
            newDesign["name"] = pName;
            newDesign["data"] = pData;
            newDesign["fireDuration"] = 0;
            newDesign["fireDate"] = null;
            newDesign["arcadeDuration"] = 0;
            newDesign["arcadeDate"] = null;

            try
            {
                await newDesign.SaveAsync().ContinueWith(t =>
                {                    
                    var query = ParseObject.GetQuery("Designs");

                    query.OrderByDescending("updatedAt").FirstAsync().ContinueWith(t2 => {
                        ParseObject result = t2.Result;
                        this.requestDesignDataForID((string)result.ObjectId, true);                               
                    });
                    
                });
            }
            catch (ParseException) {
                EventHandler<TennisEventArgs> handler = designCreationFailed_EventHandler;
                if (handler != null)
                {
                    TennisEventArgs args = new TennisEventArgs();
                    args.FinishedSuccessfully = false;
                    handler(this, args);
                }
            }            
        }

        public async void requestDesignDataForID(String pID, bool pIsNew)
        {
            String responseJson = "";
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Designs");
            await query.GetAsync(pID).ContinueWith(t =>{
                ParseObject result = t.Result;
                String id = (string)result.ObjectId;
                String name = result.Get<string>("name");
                String updatedAt = ((DateTime)result.UpdatedAt).ToLocalTime().ToString();
                String data = result.Get<string>("data");
                float fireDuration = result.Get<float>("fireDuration");
                float arcadeDuration = result.Get<float>("arcadeDuration");
                String fireDate;
                String arcadeDate;

                try{
                    fireDate = result.Get<DateTime>("fireDate").ToString();
                }
                catch(NullReferenceException){
                    fireDate = "Sin Definir";
                }

                try
                {
                    arcadeDate = result.Get<DateTime>("arcadeDate").ToString();
                }
                catch (NullReferenceException)
                {
                    arcadeDate = "Sin Definir";
                }

                responseJson = "{\"ID\":\"" + id + "\",\"Name\":\"" + name + "\",\"UpdatedAt\":\"" + 
                    updatedAt + "\",\"DesignData\":" + data + ",\"FireModeDrawingDuration\":" + 
                    fireDuration.ToString().Replace(',', '.') + ",\"FireModeBestDurationDate\":\"" + 
                    fireDate + "\",\"ArcadeModeDrawingDuration\":" + arcadeDuration.ToString().Replace(',', '.') + 
                    ",\"ArcadeModeBestDurationDate\":\"" + arcadeDate + "\"}";
            }) ;

            EventHandler<TennisEventArgs> handler = designLoadFinished_EventHandler;
            if (handler != null)
            {
                TennisEventArgs args = new TennisEventArgs();
                args.IsNewDesign = pIsNew;
                args.ParseJSONData = responseJson;
                args.FinishedSuccessfully = true;
                handler(this, args);
            }
        }


        public async void updateDesignData(String pDesignData, String pDesignId)
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Designs");
            ParseObject design = await query.GetAsync(pDesignId);
            design["data"] = pDesignData;
            await design.SaveAsync();
        }

        public async void updateDesignDuration(String pDesignId, double pDuration, DateTime pDate, String pMode)
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Designs");
            ParseObject design = await query.GetAsync(pDesignId);
            design[pMode + "Duration"] = pDuration;
            design[pMode + "Date"] = pDate;
            await design.SaveAsync();
        }

        //Event Handlers
        public event EventHandler<TennisEventArgs> designsDataFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationFailed_EventHandler;
        public event EventHandler<TennisEventArgs> designLoadFinished_EventHandler;
        /*========================*/

        private static ParseDataAccess instance;
    }
}

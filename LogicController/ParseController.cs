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
    public class ParseController
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsDataFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationFailed_EventHandler;
        public event EventHandler<TennisEventArgs> designLoadFinished_EventHandler;
        /*========================*/

        private static ParseController instance;
        private ParseController() { }

        public static ParseController Instance()
        {
            if (instance == null)
            {
                instance = new ParseController();
            }
            return instance;
        }

        public async void fetchAllDesignsMainInfo()
        {
            String jsonResponse = "[";
            var query = ParseObject.GetQuery("Designs");
            await query.FindAsync().ContinueWith(t =>
            {
                bool success = true;
                try
                {
                    IEnumerable<ParseObject> results = t.Result;
                    foreach (var obj in results){
                        jsonResponse += "{\"id\":\"" + (string)obj.ObjectId + "\", \"name\":\"" + obj.Get<string>("name") + "\",\"createdAt\":\"" + obj.CreatedAt.ToString() + "\",\"data\":null},";                        
                    }
                    jsonResponse += "]";
                    Console.WriteLine(jsonResponse);
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
            try
            {
                await newDesign.SaveAsync().ContinueWith(t =>
                {                    
                    var query = ParseObject.GetQuery("Designs");

                    query.OrderByDescending("createdAt").FirstAsync().ContinueWith(t2 => {
                        ParseObject result = t2.Result;
                        this.fetchDesignDataForID((string)result.ObjectId, true);                               
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

        public async void fetchDesignDataForID(String pID, bool pIsNew)
        {
            String responseJson = "";
            ParseQuery<ParseObject> query = ParseObject.GetQuery("Designs");
            await query.GetAsync(pID).ContinueWith(t =>{
                ParseObject result = t.Result;
                String id = (string)result.ObjectId;
                String name = result.Get<string>("name");
                String createdAt = result.CreatedAt.ToString();
                String data = result.Get<string>("data");
                responseJson = "{\"id\":\"" + id + "\",\"name\":\"" + name + "\",\"createdAt\":\"" + createdAt + "\",\"data\":" + data + "}";
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.Diagnostics;
using Tennis.TEventArgs;

namespace Tennis.ParseHandler
{
    public class TParse
    {
        //Event Handlers
        public event EventHandler<TennisEventArgs> designsDataFinishedDownloading_EventHandler;
        public event EventHandler<TennisEventArgs> designCreationFinished_EventHandler;
        /*========================*/

        private static TParse instance;
        private TParse() { }

        public static TParse Instance()
        {
            if (instance == null)
            {
                instance = new TParse();
            }
            return instance;
        }

        public async void fetchAllDesignsMainInfo()
        {
            //createTestObject();
            List<object> dataList = new List<object>();
            var query = ParseObject.GetQuery("Designs");
            await query.FindAsync().ContinueWith(t =>
            {
                bool success = true;
                try
                {
                    IEnumerable<ParseObject> results = t.Result;
                    foreach (var obj in results)
                    {
                        var id = obj.ObjectId;
                        Dictionary<string, string[]> info = new Dictionary<string, string[]>();
                        info.Add((string)id, new string[2] { obj.Get<string>("name"), obj.CreatedAt.ToString() });
                        dataList.Add(info);
                    }
                }
                catch (AggregateException) { success = false; }
                finally
                {
                    EventHandler<TennisEventArgs> handler = designsDataFinishedDownloading_EventHandler;
                    if (handler != null)
                    {
                        TennisEventArgs args = new TennisEventArgs(dataList);
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
            newDesign["data"] = pData; //TODO: JSON Encoded String with points and lines
            try
            {
                await newDesign.SaveAsync().ContinueWith(t =>
                {
                    Dictionary<string, string[]> createdDesign = new Dictionary<string, string[]>();
                    var query = ParseObject.GetQuery("Designs");

                    query.OrderByDescending("createdAt").FirstAsync().ContinueWith(t2 => {
                        ParseObject result = t2.Result;
                        createdDesign.Add((string)result.ObjectId, new string[2] { result.Get<string>("name"), result.CreatedAt.ToString() });
                        EventHandler<TennisEventArgs> handler = designCreationFinished_EventHandler;
                        if (handler != null)
                        {
                            TennisEventArgs args = new TennisEventArgs(createdDesign);
                            args.FinishedSuccessfully = true;
                            handler(this, args);
                        }        
                
                    });
                    
                });
            }
            catch (ParseException) {
                EventHandler<TennisEventArgs> handler = designCreationFinished_EventHandler;
                if (handler != null)
                {
                    handler(this, new TennisEventArgs(false));
                }
            }            
        }
    }
}

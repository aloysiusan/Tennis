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
                    EventHandler<TennisEventArgs> handler = designCreationFinished_EventHandler;
                    if (handler != null)
                    {                        
                        handler(this, new TennisEventArgs(true));
                    }
                });
            }
            catch (ParseException p) {
                EventHandler<TennisEventArgs> handler = designCreationFinished_EventHandler;
                if (handler != null)
                {
                    handler(this, new TennisEventArgs(false));
                }
            }            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.Diagnostics;

namespace Tennis.ParseHandler
{
    public class TParse
    {  
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

        public List<object> fetchAllDesignsMainInfo()
        {
            //createTestObject();
            List<object> dataList = new List<object>();
            var query = ParseObject.GetQuery("Designs");
            IEnumerable<ParseObject> results = query.FindAsync().Result;
            foreach (var obj in results)
            {
                var id = obj.ObjectId;
                Dictionary<string, string[]> info = new Dictionary<string, string[]>();
                info.Add((string)id, new string[2] { obj.Get<string>("name"), obj.CreatedAt.ToString() });

                dataList.Add(info);
            }

            Debug.Print(dataList.Count.ToString());
            return dataList;
        }

        public async static void createTestObject()
        {
            ParseObject test = new ParseObject("Designs");
            test["name"] = "testObject";
            await test.SaveAsync();
        }
    }
}

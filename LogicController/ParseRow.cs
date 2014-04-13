using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tennis.Design;

namespace Tennis.Parse.Rows
{
    public class ParseRow
    {
        public string id;
        public string name;
        public string updatedAt;
        public TDesign data;

        public ParseRow(string pID, string pName, string pCreatedAt, TDesign pData)
        {
            id = pID;
            name = pName;
            updatedAt = pCreatedAt;
            data = pData;
        }
    }
}

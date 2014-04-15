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
        public float fireDuration;
        public String fireDate;
        public float arcadeDuration;
        public String arcadeDate;

        public ParseRow(string pID, string pName, string pUpdatedAt, TDesign pData, float pFireDuration, String pFireDate, float pArcadeDuration, String pArcadeDate)
        {
            id = pID;
            name = pName;
            updatedAt = pUpdatedAt;
            data = pData;
            fireDuration = pFireDuration;
            fireDate = pFireDate;
            arcadeDuration = pArcadeDuration;
            arcadeDate = pArcadeDate;
        }
    }
}

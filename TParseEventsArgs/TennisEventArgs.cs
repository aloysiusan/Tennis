using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tennis.TEventArgs
{
    public class TennisEventArgs : EventArgs
    {
        private IEnumerable<object> _ParseData;
        private bool _FinishedSuccessfully;
        private Dictionary<string, string[]> _CreatedDesignData;

        public TennisEventArgs(IEnumerable<object> pData)
        {
            _ParseData = pData;
        }

        public TennisEventArgs(bool pSuccessfully)
        {
            _FinishedSuccessfully = pSuccessfully;
        }

        public TennisEventArgs(Dictionary<string, string[]> pCreatedDesignData)
        {
            _CreatedDesignData = pCreatedDesignData;
        }

        public IEnumerable<object> ParseData
        {
            get { return _ParseData; }
            set { _ParseData = value; }
        }

        public bool FinishedSuccessfully
        {
            get { return _FinishedSuccessfully; }
            set { _FinishedSuccessfully = value; }
        }

        public Dictionary<string, string[]> CreatedDesignData
        {
            get { return _CreatedDesignData; }
            set { _CreatedDesignData = value; }
        }
    }
}

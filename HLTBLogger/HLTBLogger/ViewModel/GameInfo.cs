using System;
using System.Collections.Generic;
using System.Text;

namespace HLTBLogger.ViewModel
{
    public class GameInfo
    {
        public string Name { get; set; }
        public string HLTBGameID { get; set; }
        public string ImageUrl { get; set; }

        public GameInfo()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

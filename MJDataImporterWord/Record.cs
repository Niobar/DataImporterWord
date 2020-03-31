using System;
using System.Collections.Generic;

namespace MJDataImporterWord
{
    internal class Record
    {
        public DateTime? BroadcastDateAndTime;
        public string Title;
        public string SubTitle;
        public short Odcinek;
        public short Sezon;
        public List<string> Actors = new List<string>();
        public string Text;

        public override string ToString()
        {
            string actorsListUnfolded = null;
            if (Actors != null)
            {
                actorsListUnfolded = string.Join(" ", Actors);
            }
            return BroadcastDateAndTime.ToString() + " TITLE:" + Title + " SUBTITLE:" + SubTitle + " S:" + Sezon + "E:" + Odcinek + " Actors:" + actorsListUnfolded + " Text:" + Text;
        }
    }
}
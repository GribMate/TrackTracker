using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onlab.BLL
{
    public class test_MatchTableRow
    {
        private string artist;
        private string title;
        private string mbid;

        public string Artist { get => artist; }
        public string Title { get => title; }
        public string MBID { get => mbid; }

        public test_MatchTableRow(string artist, string title, string mbid)
        {
            this.artist = artist;
            this.title = title;
            this.mbid = mbid;
        }
    }
}

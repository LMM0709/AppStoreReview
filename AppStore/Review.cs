using System;
using System.Linq;
using System.Text;

namespace AppStore
{
    public class Review
    {
        public string Title { get; set; }
        public int Stars { get; set; }
        public string Reviewer { get; set; }
        public string Version { get; set; }
        public string Date { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Title:" + Title + Environment.NewLine);

            if (Stars != 0)
                sb.Append("Stars:" + Stars + Environment.NewLine);
            
            sb.Append("Reviewer:" + Reviewer + Environment.NewLine);

            if(!string.IsNullOrEmpty(Version))
                sb.Append("Version:" + Version + Environment.NewLine);

            if (!string.IsNullOrEmpty(Date))
                sb.Append("Date:" + Date + Environment.NewLine);
            
            sb.Append("Comment:" + Comment + Environment.NewLine);
            return sb.ToString();
        }
    }
}

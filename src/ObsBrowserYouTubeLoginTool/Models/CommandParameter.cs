using System;
using System.Collections.Generic;
using System.Text;

namespace ObsBrowserYouTubeLoginTool.Models
{
    public class CommandParameter
    {
        public string LocalStateFilePath { get; set; }
        public string SrcCookieFilePath { get; set; }
        public string HostKey { get; set; }
        public string DestCookieFilePath { get; set; }
    }

}

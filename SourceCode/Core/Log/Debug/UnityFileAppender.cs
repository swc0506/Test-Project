using System.IO;
using log4net.Appender;

namespace Core.Log
{
    public class UnityFileAppender : FileAppender
    {
        public override string File
        {
            set { base.File = Path.Combine(Logger.logPath, value); }
        }
    }
}
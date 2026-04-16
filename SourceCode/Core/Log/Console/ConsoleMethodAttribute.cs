using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ConsoleMethodAttribute : Attribute
    {
        private string command;
        private string annotate;

        public string Command
        {
            get { return command; }
        }

        public string Annotate
        {
            get { return annotate; }
        }
        

        public ConsoleMethodAttribute(string command, string annotate)
        {
            this.command = command;
            this.annotate = annotate;
        }
    }
}
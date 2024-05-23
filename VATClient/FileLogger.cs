using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace VATClient
{
    public class FileLogger
    {
        public static void Log(string source, string actionName, string message)
        {
            try
            {
                /*Create Message object and assign values with log parameter*/
                MessageTemplate messageTemplate = new MessageTemplate();
                messageTemplate.Source = source;
                messageTemplate.ActionName = actionName;
                messageTemplate.Message = message;

                /*Create new parameterized thread object*/
                Thread newThread = new Thread(new ParameterizedThreadStart(FileLogger.WriteToFile));

                /*Start thread*/
                newThread.Start(messageTemplate);
            }
            catch (Exception ex)
            {
                LoggerFile(ex);
                //throw;
            }
        }

        public static void WriteToFile(object messageTemplate)
        {
            try
            {

                /*Cast message object with the value of parameter*/
                MessageTemplate msTemplate = (MessageTemplate)messageTemplate;

                /*Assign log path*/
                string path = Application.StartupPath + "\\Logs.txt";
                string curDate = DateTime.Now.ToString("yyyy-MMM-dd H:mm:ss zzz");

                /*Assign message header line*/
                string dateText = Environment.NewLine + curDate + " ,Source : " + msTemplate.Source + " , Method : " + msTemplate.ActionName;

                if (!File.Exists(path))
                {
                    // Create a file to write to.

                    File.WriteAllText(path, "");

                }
                /*Write message header line*/
                File.AppendAllText(path, dateText);
                /*Write a new line*/
                File.AppendAllText(path, Environment.NewLine);
                /*Write message*/
                File.AppendAllText(path, msTemplate.Message);
                /*Write another new line after the message*/
                File.AppendAllText(path, Environment.NewLine);
            }
            catch (Exception ex)
            {

                try
                {
                    LoggerFile(ex);
                }
                catch (Exception)
                {

                }
            }

        }
        private static void LoggerFile(Exception exp)
        {
            try
            {
                /*Cast message object with the value of parameter*/

                /*Assign log path*/
                string path = Application.StartupPath + "\\Loggers.txt";
                string curDate = DateTime.Now.ToString("yyyy-MMM-dd H:mm:ss zzz");

                /*Assign message header line*/
                string dateText = Environment.NewLine + curDate + Environment.NewLine;

                if (!File.Exists(path))
                {
                    // Create a file to write to.

                    File.WriteAllText(path, "");

                }
                /*Write message header line*/
                File.AppendAllText(path, dateText);
                /*Write a new line*/
                File.AppendAllText(path, Environment.NewLine);
                /*Write message*/
                File.AppendAllText(path, exp.StackTrace);
                /*Write another new line after the message*/
                File.AppendAllText(path, Environment.NewLine);
            }
            catch (Exception)
            {

            }
        }

        public class MessageTemplate
        {
            public string Source { get; set; }
            public string ActionName { get; set; }
            public string Message { get; set; }
        }

    }
}

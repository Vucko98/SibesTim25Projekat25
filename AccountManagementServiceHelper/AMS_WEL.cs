using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AccountManagementServiceHelper
{
    public class AMS_WEL : AMS, IDisposable
    {
		private static EventLog customLog = null;
		const string SourceName = "AuditSecurityManager";
		const string LogName = "AMS";

		static AMS_WEL()
		{
			try
			{
				if (!EventLog.SourceExists(SourceName))
				{
					EventLog.CreateEventSource(SourceName, LogName);
				}
				customLog = new EventLog(LogName, Environment.MachineName, SourceName);
			}
			catch (Exception e)
			{
				customLog = null;
				Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
			}
		}

		public override void Write(string dogadjaj)
        {
			if (customLog != null)
			{
				customLog.WriteEntry(dogadjaj);
			}
			else
			{
				throw new ArgumentException(string.Format("Error while trying to write event to event log."));
			}
		}

        public override string[] Read()
        {
			try
            {
				List<string> linije = new List<string>();
				EventLog[] remoteEventLogs;

				remoteEventLogs = EventLog.GetEventLogs(Environment.MachineName);
				
				EventLog ams = new EventLog();
				foreach (EventLog log in remoteEventLogs)
				{
					if (log.Log == "AMS")
						ams = log;											
				}
				EventLogEntryCollection dogadjaji = ams.Entries;								
				foreach(EventLogEntry dogadjaj in dogadjaji)
                {
					double sekunde = (DateTime.Now - dogadjaj.TimeWritten).TotalSeconds;
					if (sekunde < 5)
                    {						
						linije.Add(dogadjaj.Message);						
					}						
				}

				return linije.ToArray();
			}
			catch(Exception e)
            {
				Console.WriteLine(e);
            }

			return null;
		}

        public void Dispose()
        {
			if (customLog != null)
			{
				customLog.Dispose();
				customLog = null;
			}
		}
    }
}

using System.IO;

namespace AccountManagementServiceHelper
{
    public class AMS_TXT : AMS
    {
        public override void Write(string dogadjaj)
        {
            lock (resourceLock)
            {
                using (StreamWriter file = new StreamWriter("txtTempLog.txt", true))
                {
                    file.WriteLine(dogadjaj);
                }
            }            
        }

        public override string[] Read()
        {
            string[] lines;
            lock (resourceLock)
            {
                lines = File.ReadAllLines("txtTempLog.txt");

                using (StreamWriter file = new StreamWriter("txtLog.txt", true))
                {
                    foreach(string line in lines)
                        file.WriteLine(line);
                }
                                  
                using (StreamWriter file = new StreamWriter("txtTempLog.txt"));                                                    
            }
            return lines;
        }
    }
}

namespace AccountManagementServiceHelper
{
    public abstract class AMS
    {
        protected static object resourceLock = new object();
        public abstract void Write(string dogadjaj);
        public abstract string[] Read();
    }
}

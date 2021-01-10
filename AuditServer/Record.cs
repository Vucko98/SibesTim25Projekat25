namespace AuditServer
{
    public class Record
    {
        public int UspesnihTransakcija { get; set; }

        public int NeuspesnihTransakcija { get; set; }

        public Record()
        {
            UspesnihTransakcija = 0;
            NeuspesnihTransakcija = 0;
        }
    }
}

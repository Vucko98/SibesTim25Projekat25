namespace AccountManagementService
{
    public class Korisnik
	{
		string naziv = string.Empty;
		int racun = 0;

		public Korisnik(string _naziv, int iznos)
		{
			this.naziv = _naziv;
			this.racun = iznos;
		}

		public string Naziv
		{
			get { return naziv; }
			set { naziv = value; }
		}

		public int Racun
		{
			get { return racun; }
			set { racun = value; }
		}
	}
}

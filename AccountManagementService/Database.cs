using System.Collections.Generic;

namespace AccountManagementService
{
    public class Database
    {
        internal static Dictionary<string, Korisnik> korisnici = new Dictionary<string, Korisnik>();

        static Database()
        {
            Korisnik k1 = new Korisnik("client1", 0);
            korisnici.Add(k1.Naziv, k1);

            k1.Naziv = "client2";
            korisnici.Add(k1.Naziv, k1);

            //k1.Naziv = "wcfclient";
            //korisnici.Add(k1.Naziv, k1);
        }
    }
}

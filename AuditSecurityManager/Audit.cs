using System;

namespace AuditSecurityManager
{
    public class Audit
	{
		public static string UplataSuccess(string userName, int iznos)
		{
			string UserUplataSuccess = AuditEvents.UplataSuccess;
			string message = String.Format(UserUplataSuccess, userName, iznos);
			return message;			
		}		
		public static string UplataFailed(string userName, string razlog)
		{
			string UserUplataFailed = AuditEvents.UplataFailed;
			string message = String.Format(UserUplataFailed, userName, razlog);
			return message;
		}		

		public static string IsplataSuccess(string userName, int iznos)
		{
			string UserIsplataSuccess = AuditEvents.IsplataSuccess;
			string message = String.Format(UserIsplataSuccess, userName, iznos);
			return message;
		}		
		public static string IsplataFailed(string userName, string razlog)
		{
			string UserIsplataFailed = AuditEvents.IsplataFailed;
			string message = String.Format(UserIsplataFailed, userName, razlog);
			return message;
		}

		public static string ObrisiKorisnikaSuccess(string userName, string naziv)
		{
			string AdminObrisiKorisnikaSuccess = AuditEvents.ObrisiKorisnikaSuccess;
			string message = String.Format(AdminObrisiKorisnikaSuccess, userName, naziv);
			return message;
		}
		public static string ObrisiKorisnikaFailed(string userName, string naziv, string razlog)
		{
			string AdminObrisiKorisnikaFailed = AuditEvents.ObrisiKorisnikaFailed;
			string message = String.Format(AdminObrisiKorisnikaFailed, userName, naziv, razlog);
			return message;
		}

		public static string DodajKorisnikaSuccess(string userName, string naziv)
		{
			string AdminDodajKorisnikaSuccess = AuditEvents.DodajKorisnikaSuccess;
			string message = String.Format(AdminDodajKorisnikaSuccess, userName, naziv);
			return message;
		}
		public static string DodajKorisnikaFailed(string userName, string naziv, string razlog)
		{
			string AdminDodajKorisnikaFailed = AuditEvents.DodajKorisnikaFailed;
			string message = String.Format(AdminDodajKorisnikaFailed, userName, naziv, razlog);
			return message;
		}
	}
}

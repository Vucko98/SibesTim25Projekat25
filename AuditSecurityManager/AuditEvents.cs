using System.Reflection;
using System.Resources;

namespace AuditSecurityManager
{
    public enum AuditEventTypes
	{
		UplataSuccess = 0,
		UplataFailed = 1,

		IsplataSuccess = 2,
		IsplataFailed = 3,

		DodajKorisnikaSuccess = 4,
		DodajKorisnikaFailed = 5,

		ObrisiKorisnikaSuccess = 6,
		ObrisiKorisnikaFailed = 7,
	}

	public class AuditEvents
	{
		private static ResourceManager resourceManager = null;
		private static object resourceLock = new object();

		private static ResourceManager ResourceMgr
		{
			get
			{
				lock (resourceLock)
				{
					if (resourceManager == null)
					{
						resourceManager = new ResourceManager(typeof(AuditEventFile).ToString(), Assembly.GetExecutingAssembly());
					}
					return resourceManager;
				}
			}
		}

		public static string UplataSuccess{
			get { return ResourceMgr.GetString(AuditEventTypes.UplataSuccess.ToString()); }
		}

		public static string UplataFailed{
			get { return ResourceMgr.GetString(AuditEventTypes.UplataFailed.ToString()); }
		}

		public static string IsplataSuccess{
			get { return ResourceMgr.GetString(AuditEventTypes.IsplataSuccess.ToString()); }
		}

		public static string IsplataFailed{
			get { return ResourceMgr.GetString(AuditEventTypes.IsplataFailed.ToString()); }
		}

		public static string DodajKorisnikaSuccess{
			get { return ResourceMgr.GetString(AuditEventTypes.DodajKorisnikaSuccess.ToString()); }
		}

		public static string DodajKorisnikaFailed{
			get { return ResourceMgr.GetString(AuditEventTypes.DodajKorisnikaFailed.ToString()); }
		}

		public static string ObrisiKorisnikaSuccess{
			get { return ResourceMgr.GetString(AuditEventTypes.ObrisiKorisnikaSuccess.ToString()); }
		}

		public static string ObrisiKorisnikaFailed{
			get { return ResourceMgr.GetString(AuditEventTypes.ObrisiKorisnikaFailed.ToString()); }
		}
	}
}

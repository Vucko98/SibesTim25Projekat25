﻿using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace CertificateManager
{
	public class ClientCertValidator : X509CertificateValidator
	{
		/// <summary>
		/// Implementation of a custom certificate validation on the client side.
		/// Client should consider certificate valid if the given certifiate is not self-signed.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		/// 
		//klijent verifikuje servis, servis je issued sam od sebe
		public override void Validate(X509Certificate2 certificate)
		{
			if (certificate.Subject.Equals(certificate.Issuer))
			{
				throw new Exception("Certificate is self-issued.");
			}
		}
	}
}

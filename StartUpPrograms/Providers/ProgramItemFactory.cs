using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using StartUpPrograms.ViewModels;

namespace StartUpPrograms.Providers
{
	partial class ProgramItemFactory : IProgramItemFactory
	{

		public ProgramItemViewModel Create(string filePath, string arguments, AutoRunType type)
		{
			var hasCert = false;
			var company = string.Empty;
			try
			{
				var certificate = X509Certificate.CreateFromSignedFile(filePath);
				hasCert = true;
				company = GetCompany(certificate.Subject);
			}
			catch (CryptographicException)
			{
				if (File.Exists(filePath))
				{
					var info = FileVersionInfo.GetVersionInfo(filePath);
					company = info.CompanyName;
				}
			}

			return new ProgramItemViewModel
			{
				Arguments = arguments,
				AutoRunType = type,
				FullFilePath = filePath,
				Company = company,
				HasCertificate = hasCert,
				Name = Path.GetFileNameWithoutExtension(filePath)
			};
		}

		private string GetCompany(string subject)
		{
			return Regex.Match(subject, "(CN=\"(?<company>.+?)\")|(CN=(?<company>.+?)\\,)")
				.Groups["company"]
				.Value;
		}
	}
}
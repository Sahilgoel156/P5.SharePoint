using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace P5.SharePoint.Core.Options
{
    public class AuthenticationOptions
    {
        public const string SectionName = "MicrosoftIdentity";

        /// <summary>
        /// instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; set; } = "https://login.microsoftonline.com/{0}";

        /// <summary>
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application)
        /// </summary>
        public string Tenant { get; set; }

        /// <summary>
        /// URL of the authority
        /// </summary>
        public string Authority => string.Format(CultureInfo.InvariantCulture, Instance, Tenant);

        /// <summary>
        /// Client Id for access to Azure AD
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Certificate description for access to Azure AD
        /// </summary>
        public CertificateDescription CertificateDescription { get; set; }
    }
}

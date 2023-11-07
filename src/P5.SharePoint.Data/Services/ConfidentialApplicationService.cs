using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using P5.SharePoint.Core.Options;
using VirtoCommerce.Platform.Core.Exceptions;

namespace P5.SharePoint.Data.Services
{
    public class ConfidentialApplicationService
    {
        private readonly AuthenticationOptions _authenticationOptions;
        private readonly ICertificateLoader _certificateLoader;
        private readonly ILogger<ConfidentialApplicationService> _logger;

        public ConfidentialApplicationService(
            IOptions<AuthenticationOptions> authenticationOptions,
            ICertificateLoader certificateLoader,
            ILogger<ConfidentialApplicationService> logger)
        {
            _authenticationOptions = authenticationOptions.Value;
            _certificateLoader = certificateLoader;
            _logger = logger;
        }

        public IConfidentialClientApplication GetClientApplication()
        {
            if (string.IsNullOrEmpty(_authenticationOptions.ClientId))
            {
                throw new PlatformException("Authorization client ID is not set.");
            }
            try
            {
                _certificateLoader.LoadIfNeeded(_authenticationOptions.CertificateDescription);
            }
            catch (Exception ex)
            {
                const string message = "Error loading certificate for access to Azure AD";
                _logger.LogError(ex, message);
                throw new PlatformException($"{message}. {ex.ExpandExceptionMessage()}");
            }

            if (_authenticationOptions.CertificateDescription?.Certificate == null)
            {
                throw new PlatformException("Authorization certificate is not defined.");
            }

            var clientApp = ConfidentialClientApplicationBuilder
                .Create(_authenticationOptions.ClientId)
                .WithCertificate(_authenticationOptions.CertificateDescription.Certificate)
                .WithAuthority(_authenticationOptions.Authority)
                .Build();

            return clientApp;
        }
    }
}

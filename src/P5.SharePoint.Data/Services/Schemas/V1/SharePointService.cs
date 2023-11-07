using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Options;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Settings;
using SP = Microsoft.SharePoint.Client;

namespace P5.SharePoint.Data.Services.Schemas.V1
{
    public partial class SharePointService : BaseSharePointService
    {
        private readonly SharePointOptions _sharePointOptions;
        public SharePointService(
            ConfidentialApplicationService confidentialService,
            IOptions<SharePointOptions> sharePointOptions,
            IPlatformMemoryCache memoryCache,
            ISettingsManager settingsManager,
            IEventPublisher eventPublisher,
            ILoggerFactory loggerFactory)
            : base(confidentialService, sharePointOptions, memoryCache, settingsManager, eventPublisher, loggerFactory)
        {
            _sharePointOptions = sharePointOptions.Value;
        }


        protected override SpListItem ToListItem(SP.ListItem item)
        {
            var listItem = base.ToListItem(item);

            listItem.ContentType = listItem.ContentTypeId = Convert.ToString(item["ContainerDocumentSetName"]);

            if (listItem.FileId.HasValue)
            {
                listItem.FileContentType = Convert.ToString(item["OriginalFileContentType"]);
            }

            return listItem;
        }
    }
}

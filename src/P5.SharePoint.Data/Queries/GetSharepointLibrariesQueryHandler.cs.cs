using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace P5.SharePoint.Data.Queries
{
    public class GetSharepointLibrariesQueryHandler : IRequestHandler<GetSharepointLibrariesQuery, Result<IList<Library>>>
    {
        private readonly ISharePointService _sharePointService;
        public GetSharepointLibrariesQueryHandler(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;

        }

        public virtual async Task<Result<IList<Library>>> Handle(GetSharepointLibrariesQuery request, CancellationToken cancellationToken)
        {
            var result = await _sharePointService.GetLibrariesAsync();
            return result;
        }
    }
}

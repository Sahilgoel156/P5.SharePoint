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
using VirtoCommerce.Platform.Core.GenericCrud;

namespace P5.SharePoint.Data.Queries
{
    public class GetSharepointListsQueryHandler : IRequestHandler<GetSharepointListsQuery, Result<IList<SpList>>>
    {
        private readonly ISharePointService _sharePointService;
        public GetSharepointListsQueryHandler(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }
        public virtual async Task<Result<IList<SpList>>> Handle(GetSharepointListsQuery request, CancellationToken cancellationToken)
        {
            var result = await _sharePointService.GetListsAsync();
            return result;
        }
    }
    
}

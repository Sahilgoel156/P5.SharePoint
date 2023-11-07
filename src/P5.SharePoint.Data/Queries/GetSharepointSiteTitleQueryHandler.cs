using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Services;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace P5.SharePoint.Data.Queries
{
    public class GetSharepointSiteTitleQueryHandler : IRequestHandler<GetSharepointSiteTitleQuery, Result<SpWeb>>
    {
        private readonly ISharePointService _sharePointService;
        public GetSharepointSiteTitleQueryHandler(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }
        public virtual async Task<Result<SpWeb>> Handle(GetSharepointSiteTitleQuery request, CancellationToken cancellationToken)
        {
           var result = await _sharePointService.GetSiteTitleAsync();
            return result;
        }
    }
}   

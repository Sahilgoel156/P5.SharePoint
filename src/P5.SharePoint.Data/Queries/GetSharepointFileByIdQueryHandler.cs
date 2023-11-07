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

namespace P5.SharePoint.Data.Queries
{
    public class GetSharepointFileByIdQueryHandler : IRequestHandler<GetSharepointFileByIdQuery, Result<SpFileResult>>
    {
        private readonly ISharePointService _sharePointService;
        public GetSharepointFileByIdQueryHandler(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }
        public virtual async Task<Result<SpFileResult>> Handle(GetSharepointFileByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _sharePointService.GetFileAsync(request.Id);
            return result;
        }
    }    
}

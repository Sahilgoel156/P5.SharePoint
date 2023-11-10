using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Models.Search;
using P5.SharePoint.Core.Services;

namespace P5.SharePoint.Data.Queries
{
    public class SearchSharePointQueryHandler : IRequestHandler<SearchSharePointQuery, Result<ListItemSearchResult>>
    {
        private readonly ISharePointService _sharePointService;
        public SearchSharePointQueryHandler(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }
       
        public virtual async Task<Result<ListItemSearchResult>> Handle(SearchSharePointQuery request, CancellationToken cancellationToken)
        {
            //var data = await _sharePointService.SearchListItemsAsync(new ListItemSearchCriteria() { Id = request.StoreId});
            var data = await _sharePointService.SearchListItemsAsync(new ListItemSearchCriteria() { Id = "a715e0c7-244f-48c4-9f00-762c4b7ef548" });
            return data;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Common;
using P5.SharePoint.Core.Models.Search;

namespace P5.SharePoint.Core.Services
{
    public interface ISharePointService
    {
        Task<Result<SpFileResult>> GetFileAsync(Guid id);
        Task<Result<ListItemSearchResult>> SearchListItemsAsync(ListItemSearchCriteria criteria);
        Task<Result<SpWeb>> GetSiteTitleAsync();
        Task<Result<IList<SpList>>> GetListsAsync();
        Task<Result<IList<Library>>> GetLibrariesAsync();
    }
}

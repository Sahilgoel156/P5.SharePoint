using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using P5.SharePoint.Core;
using P5.SharePoint.Core.Services;
using Microsoft.AspNetCore.Http;
using P5.SharePoint.Core.Models.Common;
using System.Collections.Generic;
using P5.SharePoint.Core.Models;
using P5.SharePoint.Core.Models.Search;
using P5.SharePoint.Web.Extension;

namespace P5.SharePoint.Web.Controllers.Api
{
    [Route("api/sharepoint")]
    public class SharePointController : Controller
    {
        private readonly ISharePointService _sharePointService;

        public SharePointController(ISharePointService sharePointService)
        {
            _sharePointService = sharePointService;
        }

        /// <summary>
        /// Get site metadata
        /// </summary>
        [HttpGet]
        [Route("title")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        [ProducesResponseType(typeof(Result<SpWeb>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSiteTitle()
        {
            var result = await _sharePointService.GetSiteTitleAsync();
            return result.Succeeded ? (IActionResult)Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get site lists
        /// </summary>
        [HttpGet]
        [Route("lists")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        [ProducesResponseType(typeof(Result<IList<SpList>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLists()
        {
            var result = await _sharePointService.GetListsAsync();
            return result.Succeeded ? (IActionResult)Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get libraries data
        /// </summary>
        [HttpGet]
        [Route("libraries")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        [ProducesResponseType(typeof(Result<IList<Library>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLibraries()
        {
            var result = await _sharePointService.GetLibrariesAsync();
            return result.Succeeded ? (IActionResult)Ok(result) : BadRequest(result) ;
        }

        /// <summary>
        /// Get file by id
        /// </summary>
        /// <param name="id">File identifier</param>
        [HttpGet]
        [Route("file/{id:guid}")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<IActionResult> GetFile([FromRoute] Guid id)
        {
            var result = await _sharePointService.GetFileAsync(id);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            var value = result.Value;
            return File(value.Stream, value.ContentType, value.Name,
                value.LastModified, !string.IsNullOrEmpty(value.ETag) ? EntityTagHeaderValue.Parse(value.ETag) : null);
        }

        [HttpPost]
        [Route("search")]
        [Authorize(ModuleConstants.Security.Permissions.Read)]
        public async Task<ActionResult<Result<ListItemSearchResult>>> SearchListItems([FromBody] ListItemSearchCriteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return Result.Failed<ListItemSearchResult>(ModelState.GetErrors());
            }

            return await _sharePointService.SearchListItemsAsync(criteria ?? new ListItemSearchCriteria());
        }
    }
}

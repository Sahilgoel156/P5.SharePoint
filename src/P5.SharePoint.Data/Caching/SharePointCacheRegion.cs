using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;

namespace P5.SharePoint.Data.Caching
{
    public class SharePointCacheRegion : CancellableCacheRegion<SharePointCacheRegion>
    {
        public static IChangeToken CreateChangeToken<TValue>(TValue value) where TValue : ValueObject
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return CreateChangeTokenForKey($"{typeof(TValue).Name}|{value.GetCacheKey()}");
        }
    }
}

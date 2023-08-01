using System;
using System.Collections.Generic;

namespace Technoshop.Common
{
    
        public class Paginate
        {
            public int PageSize { get; set; } = 5;
            public int TotalPages => (int)Math.Ceiling(((decimal)(TotalCount) / PageSize));
            public int TotalCount { get; set; }
            public int PageNumber { get; set; } = 1;

        }
}

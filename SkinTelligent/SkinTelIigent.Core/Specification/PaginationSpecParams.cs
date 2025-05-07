using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class PaginationSpecParams
    {
        private string? _search;

        public string? Search
        {
            get => _search;
            set
            {
                _search = value;
                SearchWords = string.IsNullOrWhiteSpace(_search)
                    ? Array.Empty<string>()
                    : _search.ToLower().Split(' ');
            }
        }

        public string[] SearchWords { get; private set; } = Array.Empty<string>();

        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
    }
}

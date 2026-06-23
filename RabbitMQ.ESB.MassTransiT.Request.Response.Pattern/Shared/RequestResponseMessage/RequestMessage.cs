using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestResponseMessage
{
    public record RequestMessage
    {
        public int MessageNo { get; set; }
        public string? Text { get; set; }
    }
}

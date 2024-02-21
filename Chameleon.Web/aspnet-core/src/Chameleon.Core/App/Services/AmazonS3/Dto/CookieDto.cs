using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.App.Services.AmazonS3.Dto
{
    public class CookieDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Domain { get; set; }
        public decimal ExpirationDate { get; set; }
        public bool HostOnly { get; set; }
        public bool HttpOnly { get; set; }
        public string Path { get; set; }
        public string SameSite { get; set; }
        public bool Secure { get; set; }
        public bool Session { get; set; }
        public string StoreId { get; set; }
    }
}

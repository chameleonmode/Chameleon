using Abp.Auditing;
using Abp.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Models.TokenAuth
{
    public class IsLicActiveResultModel
    {
        public bool isActive { get; set; }
    }
}

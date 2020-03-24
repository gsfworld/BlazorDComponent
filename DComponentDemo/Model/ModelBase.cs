using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DComponentDemo.Model
{
    public class SysUser
    {
        public string UserId { get; set; }
        public string LoginId { get; set; }
        public string UserName { get; set; }
        public string IsOnline { get; set; }
        public string IsUse { get; set; }
        public string OrgId { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class SysOrg
    {
        public string OrgId { get; set; }
        public string OrgName { get; set; }
    }
}

using DComponentDemo.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DComponentDemo.Data
{
    public class BaseHelper
    {
        public static List<SysUser> GetDemoUser()
        {
            return new List<SysUser>{
            new SysUser
            {
                UserId=Guid.NewGuid().ToString(),
                LoginId="D1",
                UserName="张1",
                IsOnline="N",
                IsUse="Y",
                OrgId="o1",
                CreateDate=DateTime.Now
            },
            new SysUser
            {
                UserId=Guid.NewGuid().ToString(),
                LoginId="D2",
                UserName="张2",
                IsOnline="N",
                IsUse="Y",
                OrgId="o1",
                CreateDate=DateTime.Now
            },
            new SysUser
            {
                UserId=Guid.NewGuid().ToString(),
                LoginId="D3",
                UserName="张3",
                IsOnline="N",
                IsUse="Y",
                OrgId="o2",
                CreateDate=DateTime.Now
            },
            new SysUser
            {
                UserId=Guid.NewGuid().ToString(),
                LoginId="D4",
                UserName="张4",
                IsOnline="N",
                IsUse="Y",
                OrgId="o2",
                CreateDate=DateTime.Now
            }
            };
        }

        public static List<SysOrg> GetDemoOrg()
        {
            return new List<SysOrg>
           {
            new SysOrg
            {
                OrgId="o1",
                OrgName="测试部门1"
            },
            new SysOrg
            {
                OrgId="o2",
                OrgName="测试部门2"
            }
           };
        }

        public static List<SysUser> GetDemoUserTree()
        {
            var user = GetDemoUser();
            return user.Union(GetDemoOrg().Select(p => new SysUser
            {
                UserId=p.OrgId,
                UserName = p.OrgName,
                OrgId=null
            })).ToList();
        }
    }
}


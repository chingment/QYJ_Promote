﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class AdminServiceFactory : BaseFactory
    {
        public static WithdrawProvider Withdraw
        {
            get
            {
                return new WithdrawProvider();
            }
        }

        public static SysUserProvider SysUser
        {
            get
            {
                return new SysUserProvider();
            }
        }

        public static SysStaffUserProvider SysStaffUser
        {
            get
            {
                return new SysStaffUserProvider();
            }
        }

        public static SysRoleProvider SysRole
        {
            get
            {
                return new SysRoleProvider();
            }
        }

        public static SysMenuProvider SysMenu
        {
            get
            {
                return new SysMenuProvider();
            }
        }

        public static SysItemCacheUpdateTimeProvider SysItemCacheUpdateTime
        {
            get
            {
                return new SysItemCacheUpdateTimeProvider();
            }
        }

        public static AuthorizeRelayProvider AuthorizeRelay
        {
            get
            {
                return new AuthorizeRelayProvider();
            }
        }

        public static BackgroundJobProvider BackgroundJob
        {
            get
            {
                return new BackgroundJobProvider();
            }
        }
    }
}

using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.DAL
{

    public class LumosDbContext : AuthorizeRelayDbContext
    {
        public IDbSet<Promote> Promote { get; set; }
        //public IDbSet<PromoteCoupon> PromoteCoupon { get; set; }

        public IDbSet<PromoteSku> PromoteSku { get; set; }
        public IDbSet<PromoteUser> PromoteUser { get; set; }
        public IDbSet<PromoteUserRelation> PromoteUserRelation { get; set; }
        public IDbSet<ProductSku> ProductSku { get; set; }
        public IDbSet<Order> Order { get; set; }
        public IDbSet<OrderDetails> OrderDetails { get; set; }
        public IDbSet<WxUserInfo> WxUserInfo { get; set; }
        public IDbSet<WxMsgPushLog> WxMsgPushLog { get; set; }
        public IDbSet<WxAutoReply> WxAutoReply { get; set; }
        public IDbSet<Fund> Fund { get; set; }
        public IDbSet<FundTrans> FundTrans { get; set; }
        public IDbSet<PromoteRefereerRewardSet> PromoteRefereerRewardSet { get; set; }
        public IDbSet<OrderNotifyLog> OrderNotifyLog { get; set; }

        public IDbSet<ClientCoupon> ClientCoupon { get; set; }

        public IDbSet<ClientAccessLog> ClientAccessLog { get; set; }
        public IDbSet<ClientShareLog> ClientShareLog { get; set; }

        public IDbSet<Withdraw>  Withdraw { get; set; }

        public IDbSet<PromoteBlackList> PromoteBlackList { get; set; }

        //public FxDbContext()
        //    : base("DefaultConnection")
        //{
        //   // this.Configuration.ProxyCreationEnabled = false;
        //}

        public IDbSet<BizSn> BizSn { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }


    public class FxContextDatabaseInitializerForCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<LumosDbContext>
    {
        protected override void Seed(LumosDbContext context)
        {
            base.Seed(context);
        }

        public List<SysPermission> GetPermissionList(PermissionCode permission)
        {
            Type t = permission.GetType();
            List<SysPermission> list = new List<SysPermission>();
            list = GetPermissionList(t, list);
            return list;
        }

        private List<SysPermission> GetPermissionList(Type t, List<SysPermission> list)
        {
            if (t.Name != "Object")
            {
                System.Reflection.FieldInfo[] properties = t.GetFields();
                foreach (System.Reflection.FieldInfo property in properties)
                {
                    string pId = "0";
                    object[] typeAttributes = property.GetCustomAttributes(false);
                    foreach (PermissionCodeAttribute attribute in typeAttributes)
                    {
                        pId = attribute.PId;
                    }
                    object id = property.GetValue(null);
                    string name = property.Name;
                    SysPermission model = new SysPermission();
                    model.Id = id.ToString();
                    model.Name = name;
                    model.PId = pId;
                    list.Add(model);
                }
                list = GetPermissionList(t.BaseType, list);
            }
            return list;
        }

    }

}

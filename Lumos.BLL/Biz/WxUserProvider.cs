using Lumos.DAL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Lumos.Mvc;
using Newtonsoft.Json;

namespace Lumos.BLL
{
    public class WxUserProvider : BaseProvider
    {
        private static readonly object goSettlelock = new object();

        public WxUserInfo CheckedUser(int operater, WxUserInfo userInfo,bool isAuthorizationSubscribe=false)
        {
            WxUserInfo mod_UserInfo = null;
            //LogUtil("开始检测用户信息：{0}", JsonConvert.SerializeObject(userInfo));
            //lock (goSettlelock)
            //{
            //    try
            //    {
            //        using (TransactionScope ts = new TransactionScope())
            //        {
            //            mod_UserInfo = CurrentDb.WxUserInfo.Where(m => m.OpenId == userInfo.OpenId).FirstOrDefault();
            //            if (mod_UserInfo == null)
            //            {
            //                var sysClientUser = new SysClientUser();
            //                sysClientUser.UserName = string.Format("wx{0}", Guid.NewGuid().ToString().Replace("-", ""));
            //                sysClientUser.PasswordHash = PassWordHelper.HashPassword("888888");
            //                sysClientUser.SecurityStamp = Guid.NewGuid().ToString();
            //                sysClientUser.RegisterTime = this.DateTime;
            //                sysClientUser.CreateTime = this.DateTime;
            //                sysClientUser.Creator = operater;
            //                sysClientUser.Status = Enumeration.UserStatus.Normal;
            //                CurrentDb.SysClientUser.Add(sysClientUser);
            //                CurrentDb.SaveChanges();

            //                mod_UserInfo = new WxUserInfo();
            //                mod_UserInfo.UserId = sysClientUser.Id;
            //                mod_UserInfo.OpenId = userInfo.OpenId;
            //                mod_UserInfo.AccessToken = userInfo.AccessToken;
            //                mod_UserInfo.ExpiresIn = userInfo.ExpiresIn;
            //                mod_UserInfo.Nickname = userInfo.Nickname;
            //                mod_UserInfo.Sex = userInfo.Sex;
            //                mod_UserInfo.Province = userInfo.Province;
            //                mod_UserInfo.City = userInfo.City;
            //                mod_UserInfo.Country = userInfo.Country;
            //                mod_UserInfo.HeadImgUrl = userInfo.HeadImgUrl;
            //                mod_UserInfo.UnionId = userInfo.UnionId;
            //                mod_UserInfo.CreateTime = this.DateTime;
            //                mod_UserInfo.Creator = operater;
            //                mod_UserInfo.Mobile = userInfo.Mobile;
            //                mod_UserInfo.Referee = userInfo.Referee;
            //                mod_UserInfo.NearStoreId = userInfo.NearStoreId;

            //                if (isAuthorizationSubscribe)//通过“允许XXX获取用户资料订阅”
            //                {
            //                    userInfo.InfoFrom = Enumeration.WxUserInfoFrom.Subscribe;
            //                    userInfo.IsSubscribed = true;
            //                }

            //                if (userInfo.InfoFrom == Enumeration.WxUserInfoFrom.Subscribe)
            //                {
            //                    mod_UserInfo.IsSubscribed = userInfo.IsSubscribed;
            //                }

            //                mod_UserInfo.InfoFrom = userInfo.InfoFrom;
            //                CurrentDb.WxUserInfo.Add(mod_UserInfo);
            //                CurrentDb.SaveChanges();
            //            }
            //            else
            //            {
            //                mod_UserInfo.AccessToken = userInfo.AccessToken;
            //                mod_UserInfo.ExpiresIn = userInfo.ExpiresIn;
            //                mod_UserInfo.Nickname = userInfo.Nickname;
            //                mod_UserInfo.Sex = userInfo.Sex;
            //                mod_UserInfo.Province = userInfo.Province;
            //                mod_UserInfo.City = userInfo.City;
            //                mod_UserInfo.Country = userInfo.Country;
            //                mod_UserInfo.HeadImgUrl = userInfo.HeadImgUrl;
            //                mod_UserInfo.UnionId = userInfo.UnionId;
            //                mod_UserInfo.InfoFrom = userInfo.InfoFrom;
            //                mod_UserInfo.LastUpdateTime = this.DateTime;
            //                mod_UserInfo.Mender = operater;


            //                if (mod_UserInfo.NearStoreId == 0)
            //                {
            //                    mod_UserInfo.NearStoreId = userInfo.NearStoreId;
            //                }

            //                if (userInfo.InfoFrom == Enumeration.WxUserInfoFrom.Subscribe)
            //                {
            //                    mod_UserInfo.IsSubscribed = userInfo.IsSubscribed;
            //                }
            //            }

            //            if (userInfo.InfoFrom == Enumeration.WxUserInfoFrom.Subscribe)
            //            {
            //                if (userInfo.IsSubscribed)
            //                {
            //                    if (mod_UserInfo.FirstSubTime == null)
            //                    {
            //                        if (userInfo.FirstSubTime != null)
            //                        {
            //                            Log.InfoFormat("关注时间（userInfo.FirstSubTime.Value）：{0}", userInfo.FirstSubTime.Value);
            //                        }
            //                        else
            //                        {
            //                            Log.InfoFormat("关注时间（this.DateTime）：{0}", this.DateTime);
            //                        }
            //                        //第一次关注，记录推荐人

            //                        mod_UserInfo.Referee = userInfo.Referee;
            //                        mod_UserInfo.FirstSubTime = userInfo.FirstSubTime ?? this.DateTime;
            //                        //mod_UserInfo.NearStoreId = userInfo.NearStoreId;
            //                        //实现自动送红包


            //                        string[] couponCodes = CommonAppSettingsUtil.GetWxSubCouponTmplCodes();

            //                        if (couponCodes != null)
            //                        {
            //                            if (couponCodes.Length > 0)
            //                            {
            //                                foreach (var couponCode in couponCodes)
            //                                {
            //                                    Log.InfoFormat("系统自动领取优惠卷:{0}，领取到用户:{1}", couponCode, mod_UserInfo.UserId);
            //                                    BizFactory.Coupon.Receive(operater, Enumeration.CouponSourceType.Receive, couponCode, mod_UserInfo.UserId, "关注领取");
            //                                }
            //                            }
            //                        }

            //                    }
            //                    else
            //                    {
            //                        mod_UserInfo.LastSubTime = userInfo.LastSubTime;
            //                    }
            //                }
            //                else
            //                {
            //                    if (mod_UserInfo.FirstCancleSubTime == null)
            //                    {
            //                        mod_UserInfo.FirstCancleSubTime = userInfo.FirstCancleSubTime;
            //                    }
            //                    else
            //                    {
            //                        mod_UserInfo.LastCancleSubTime = userInfo.LastCancleSubTime;
            //                    }
            //                }
            //            }


            //            CurrentDb.SaveChanges();

            //            ts.Complete();

            //        }
            //        Log.InfoFormat("结束检测用户信息：{0}", userInfo.OpenId);
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Error("检查微信用户系统发生异常", ex);
            //    }
            //}

            return mod_UserInfo;
        }
    }
}

using Lumos;
using Lumos.BLL;
using Lumos.DAL;
using Lumos.Entity;
using Lumos.WeiXinSdk;
using Lumos.WeiXinSdk.MsgPush;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{

    public sealed class Cat
    {
        public event EventHandler Calling;

        public void Call()
        {
            Console.WriteLine("猫叫了...");
            if (Calling != null)//检查是否有事件注册  
                Calling(this, EventArgs.Empty);//调用事件注册的方法  
        }
    }

    public sealed class Mouser
    {
        public void Escape()
        {
            Console.WriteLine("老鼠逃跑了...");
        }

    }

    public sealed class Master
    {
        //发生猫叫时惊醒  
        public void Wakeup(object sender, EventArgs e)
        {
            Console.WriteLine("主人惊醒了...");
        }
    }


    public class A
    {
        public static int a = 1;
        public A()
        {
            a++;
            Console.WriteLine("A.a=" + a);
            Console.WriteLine("我是A");
        }

        public virtual void Show()
        {
            Console.WriteLine("我是A.Show");
        }

        public virtual void Cry()
        {
            Console.WriteLine("我是A.Cry");
        }
    }

    public class B : A
    {
        public B()
        {
            a++;
            Console.WriteLine("B.a=" + a);
            Console.WriteLine("我是B");
        }

        public override void Show()
        {
            //base.Show();
            Console.WriteLine("我是B.Show");
        }

        public new void Cry()
        {
            Console.WriteLine("我是B.Cry");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var operater = "00000000000000000000000000000000";

            string xml= "<xml><ToUserName><![CDATA[gh_fc0a06a20993]]></ToUserName><FromUserName><![CDATA[oZI8Fj040-be6rlDohc6gkoPOQTQ]]></FromUserName><CreateTime>1472551036</CreateTime><MsgType><![CDATA[event]]></MsgType><Event><![CDATA[user_get_card]]></Event><CardId><![CDATA[pZI8Fjwsy5fVPRBeD78J4RmqVvBc]]></CardId><IsGiveByFriend>0</IsGiveByFriend><UserCardCode><![CDATA[226009850808]]></UserCardCode><FriendUserName><![CDATA[]]></FriendUserName><OuterId>0</OuterId><OldUserCardCode><![CDATA[]]></OldUserCardCode><OuterStr><![CDATA[12b]]></OuterStr><IsRestoreMemberCard>0</IsRestoreMemberCard><IsRecommendByFriend>0</IsRecommendByFriend><UnionId>o6_bmasdasdsad6_2sgVt7hMZOPfL</UnionId></xml>";

            var baseEventMsg = WxMsgFactory.CreateMessage(xml);


            OAuthApi.UploadMultimediaImage("13_biDPYMhICk8L9pCaDLBYdYdCrHRpYjUSUfACRKGgr8ezw-lxqQxYLYXKTkWVwo6fKju-5XjZ675hOk7w7r3zV5I8KoqjFajap6gaJK2PAhoWujJCLf03E4j0er-ZLs3VU_1e7B69QUl-TwX_FFGhAFAYYZ", "d:\\hb1.jpg");

           // System.Drawing.Image oImg = System.Drawing.Image.FromFile("d:\\hb1.jpg");
           // System.Drawing.Image oImg1 = System.Drawing.Image.FromFile("d:\\hb2.jpg");
           // System.Drawing.Bitmap map = new Bitmap(oImg);
           // oImg.Dispose();
           // Graphics g = Graphics.FromImage(map);
           // g.InterpolationMode = InterpolationMode.HighQualityBilinear;
           // SolidBrush brush = new SolidBrush(Color.Green);
           // PointF P = new PointF(100, 100);
           // Font f = new Font("Arial", 20);
           // //g.DrawString(nickName, f, brush, 310, 542);
           // g.DrawImage(oImg1, 75, 540, 77, 77);//画二维码图片
           //// g.DrawImage(oImg3, 85, 730, 220, 220);//画二维码图片
           // map.Save("d:\\hb3.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
           // f.Dispose();
           // g.Dispose();


            //BizFactory.Order.PayCompleted(operater, "2018090214233885209742", DateTime.Now);
            //SnUtil.BulidOrderNo(Enumeration.BizSnType.Order);

            //LumosDbContext CurrentDb = new LumosDbContext();

            //var promoteId = "a999753c5fe14e26bbecad576b6a6909";
            //var userId = "4";
            //var pUserId = "3";
            //var createTime = DateTime.Now;


            //var promoteUser = CurrentDb.PromoteUser.Where(m => m.UserId == userId && m.PromoteId == promoteId).FirstOrDefault();
            //if (promoteUser == null)
            //{
            //    promoteUser = new PromoteUser();
            //    promoteUser.Id = GuidUtil.New();
            //    promoteUser.PromoteId = promoteId;
            //    promoteUser.UserId = userId;
            //    promoteUser.PUserId = pUserId;
            //    promoteUser.IsAgent = false;
            //    promoteUser.CreateTime = createTime;
            //    promoteUser.Creator = operater;
            //    CurrentDb.PromoteUser.Add(promoteUser);
            //    CurrentDb.SaveChanges();
            //}

            //var promoteUsers = CurrentDb.PromoteUser.ToList();

            //var promoteUserFathers = GetFatherList(promoteUsers, userId).Where(m => m.UserId != userId && m.IsAgent == false).Take(3).ToList();

            //for (int i = 0; i < promoteUserFathers.Count; i++)
            //{
            //    int dept = (i + 1);
            //    Console.WriteLine("用户Id: " + userId + "是用户Id:" + promoteUserFathers[i].UserId + "的" + dept + "级分销商");
            //    var promoteUserRelation = new PromoteUserRelation();
            //    promoteUserRelation.Id = GuidUtil.New();
            //    promoteUserRelation.UserId = promoteUserFathers[i].UserId;
            //    promoteUserRelation.PromoteId = promoteId;
            //    promoteUserRelation.CUserId = userId;
            //    promoteUserRelation.Dept = dept;
            //    promoteUserRelation.CreateTime = createTime;
            //    promoteUserRelation.Creator = operater;
            //    CurrentDb.PromoteUserRelation.Add(promoteUserRelation);
            //    CurrentDb.SaveChanges();

            //}

            //foreach (var item in list)
            //{
            //    //Console.WriteLine("父用户Id:" + item.UserId);

            //    //var son = GetSonList(promoteUser, item.PUserId).ToList();
            //    //for (int i = 0; i < son.Count; i++)
            //    //{
            //    //    Console.WriteLine("第" + (i + 1) + "个子用户Id:" + son[i].UserId);
            //    //}

            //}
        }

        public static IEnumerable<PromoteUser> GetFatherList(IList<PromoteUser> list, string userId)
        {
            var query = list.Where(p => p.UserId == userId).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetFatherList(list, t.PUserId)));
        }

        public static IEnumerable<PromoteUser> GetSons(IList<PromoteUser> list, string Fid)
        {
            var query = list.Where(p => p.UserId == Fid).ToList();
            var list2 = query.Concat(GetSonList(list, Fid));
            return list2;
        }

        public static IEnumerable<PromoteUser> GetSonList(IList<PromoteUser> list, string Fid)
        {
            var query = list.Where(p => p.PUserId == Fid).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.UserId)));
        }
    }
}

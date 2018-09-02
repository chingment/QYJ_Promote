using Lumos;
using Lumos.BLL;
using Lumos.DAL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
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
            BizFactory.Order.PayCompleted(operater, "2018090214233885209742", DateTime.Now);
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

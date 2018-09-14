using Lumos.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lumos.BLL.Task
{
    public class Task4CalculateProfitProvider : BaseProvider, ITask
    {
        private int ThreadCount = 1;

        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            for (int i = 0; i < 1; i++)
            {
                Thread thread = new Thread(new ThreadStart(QueueList));
                thread.Start();
            }

            return result;
        }

        public void QueueList()
        {
            ReidsMqByCalProfit redisMq = new ReidsMqByCalProfit();
            while (true)
            {
                try
                {
                    var handleObj = redisMq.Pop();
                    if (handleObj == null)
                    {
                        Console.WriteLine("无处理信息，休息100毫秒");
                        Thread.Sleep(1000);
                        continue;
                    }

                    while (ThreadCount <= 0)
                    {
                        Thread.Sleep(100);
                    }
                    ThreadCount--;

                    Thread thread = new Thread(new ThreadStart(handleObj.Handle));
                    thread.Start();

                    Console.WriteLine("正在处理信息，休息100毫秒");
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    Thread.Sleep(1000);
                }
                finally
                {
                    ThreadCount++;
                }

            }
        }
    }
}

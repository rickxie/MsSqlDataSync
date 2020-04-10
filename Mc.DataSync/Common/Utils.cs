using MiniAbp.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Sl.Bpm.CodeTool.Common
{
    public class Utils
    {

        /// <summary>
        /// 实现js setTimeout方法
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static CancellationTokenSource SetTimeout(Action callback, int delay = 100)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            Task.Delay(delay, source.Token)
                .ContinueWith(task =>
                {
                    if (!task.IsCanceled)
                    {
                        callback();
                    }
                });
            return source;
        }

       /// <summary>
       /// 实现js clearTimeout
       /// </summary>
       /// <param name="source"></param>
        public static void ClearTimeout(CancellationTokenSource source)
        {
            source.Cancel();
        }

       /// <summary>
       /// 返回一个回调,只要该方法一直被调用,那么参数回调永远不会被触发.如果该方法被停止调用,则N毫秒后,将会被触发
       /// </summary>
       /// <param name="callback"></param>
       /// <param name="delay"></param>
       /// <returns></returns>
        public static Action Debounce(Action callback, int delay = 100)
        {
            CancellationTokenSource source = null;
            return () =>
            {
                if (source != null)
                {
                    ClearTimeout(source);
                }
                source = SetTimeout(() =>
                {
                    source = null;
                    callback();
                }, delay);
            };
        }


        public static string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取服务器当前时间
        /// </summary>
        public static DateTime Now
        {
            get
            {
                DateTime now = DateTime.Now;
                using (var con = new SqlConnection(DbDapper.ConnectionString))
                {
                    con.Open();
                    now = con.ExecuteScalar<DateTime>("SELECT GETDATE();");
                }
                return now;
            }
        }
    }
}

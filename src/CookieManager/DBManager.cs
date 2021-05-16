using CookieManager.Models;
using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieManager
{
    public static class DBManager
    {
        public static IEnumerable<cookies> GetYoutubeCookies(string srcCookieFilePath, string host_key)
        {
            DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.SqliteDialect();
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(cookiesClassMapper<>);

            using (var cn = new System.Data.SQLite.SQLiteConnection("Data Source=" + srcCookieFilePath))
            {
                cn.Open();
                var predicate = Predicates.Field<cookies>(f => f.host_key, Operator.Eq, host_key);
                var list = cn.GetList<cookies>(predicate).ToArray();
                return list;
            }
        }


        public static void WriteYoutubeCookies(IEnumerable<Models.cookies> cookies, string host_key, string srcCookieFilePath, string destCookieFilePath)
        {
            var yc = GetYoutubeCookies(destCookieFilePath, host_key);

            DapperExtensions.DapperExtensions.SqlDialect = new DapperExtensions.Sql.SqliteDialect();
            DapperExtensions.DapperExtensions.DefaultMapper = typeof(cookiesClassMapper<>);
            using (var cn = new System.Data.SQLite.SQLiteConnection("Data Source=" + destCookieFilePath))
            {
                cn.Open();

                yc.ToList().ForEach(c =>
                {
                    cn.Delete(c);
                });

                cookies.ToList().ForEach(c =>
                {
                    cn.Insert(c);
                });
            }
        }
    }
}

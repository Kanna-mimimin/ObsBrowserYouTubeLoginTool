using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieManager.Models
{
    public class cookies
    {
        public long creation_utc { get; set; }
        public string host_key { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string path { get; set; }
        public long expires_utc { get; set; }
        public int is_secure { get; set; }
        public int is_httponly { get; set; }
        public long last_access_utc { get; set; }
        public int has_expires { get; set; }
        public int is_persistent { get; set; }
        public int priority { get; set; }
        public byte[] encrypted_value { get; set; }
    }

    public struct Blob
    {
        public byte[] Value;
    }

    public sealed class cookiesClassMapper<T> : DapperExtensions.Mapper.ClassMapper<T> where T : class
    {
        public cookiesClassMapper()
        {
            if (typeof(T) == typeof(cookies))
            {
                Table("cookies");
                Map(x => (x as cookies).host_key).Column("host_key").Key(DapperExtensions.Mapper.KeyType.Assigned);
                Map(x => (x as cookies).name).Column("name").Key(DapperExtensions.Mapper.KeyType.Assigned);
                Map(x => (x as cookies).path).Column("path").Key(DapperExtensions.Mapper.KeyType.Assigned);
            }
            AutoMap();
        }
    }
}

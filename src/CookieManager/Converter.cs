using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace CookieManager
{
    public static class Converter
    {
        static readonly int DPAPI_PREFIX_SIZE = "DPAPI".Length;
        const int NONCE_SIZE = 12;
        static readonly int PREFIX_SIZE = "v10".Length;
        const int TAG_SIZE = 16;

        static byte[] Unprotect(byte[] encryptedData) => ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);

        static byte[] DpapiProtect(byte[] data) => ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);



        public static byte[] DecryptAesGcm(byte[] key, byte[] nonce, byte[] ciphertext, byte[] tag)
        {
            //複号後の値を格納する配列を準備
            byte[] plain = new byte[ciphertext.Length];

            using (var cipher = new AesGcm(key))
            {
                cipher.Decrypt(nonce, ciphertext, tag, plain);
            }
            return plain;
        }

        static Models.cookies ConvertCookies(Models.cookies cookies, byte[] key)
        {
            var cs = Encoding.ASCII.GetString(cookies.encrypted_value);
            if (!cs.StartsWith("v10"))
            {
                return null;
            }

            //先頭3文字"V10"を除去してからnonceに該当する12バイト取得する
            var nonce = cookies.encrypted_value.Skip(PREFIX_SIZE).Take(NONCE_SIZE).ToArray();

            //先頭15バイト("V10"とnone)と末尾16バイト(tag)を除去
            var cipherText = cookies.encrypted_value.Take(cookies.encrypted_value.Length - TAG_SIZE).Skip(PREFIX_SIZE + NONCE_SIZE).ToArray();

            //末尾16バイトの認証タグを取得
            var tag = cookies.encrypted_value.Skip(cookies.encrypted_value.Length - TAG_SIZE).ToArray();

            //AES-GCMで複合
            var decrypted_value = DecryptAesGcm(key, nonce, cipherText, tag);

            //DPAPIで暗号
            cookies.encrypted_value  = DpapiProtect(decrypted_value);

            return cookies;
        }

        static byte[] GetKey(string localStateFilePath)
        {
            var json = System.IO.File.ReadAllText(localStateFilePath);
            var pt = "\"os_crypt\":\\{\"encrypted_key\":\"(?<key>.*?)\"\\}";
            var match = Regex.Match(json, pt);
            var stringEncryptedKey = match.Groups["key"].Value;

            var encryptedKey = System.Convert.FromBase64String(stringEncryptedKey);

            //先頭5文字"DPAPI"を除去
            encryptedKey = encryptedKey.Skip(DPAPI_PREFIX_SIZE).ToArray();

            //DPAPIで複号
            var key = Unprotect(encryptedKey);

            return key;
        }

        public static void Convert(string stateFilePath, string srcCookieFilePath, string hostKey, string destCookieFilePath)
        {
            //Keyを取得
            var key = GetKey(stateFilePath);

            //ブラウザからYouTubeのCookieを取得
            var cookies = DBManager.GetYoutubeCookies(srcCookieFilePath, hostKey);

            //クッキーを変換
            var newcookies = cookies.Select(c => ConvertCookies(c, key)).ToArray();

            //OBSブラウザのクッキーに上書き
            DBManager.WriteYoutubeCookies(newcookies, hostKey, srcCookieFilePath, destCookieFilePath);
        }

    }
}

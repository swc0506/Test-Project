using System;
using System.Text;

namespace Core
{
    public static class EncryptUtils
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="bytes">需要加密的字节</param>
        /// <returns>加密后的字节码</returns>
        public static byte[] Encrypt(string key, byte[] bytes)
        {
            if (string.IsNullOrEmpty(key))
            {
                return bytes;
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            int keyLength = keyBytes.Length;

            int length = bytes.Length;
            byte[] res = new byte[length];
            for (int i = 0; i < length; i++)
            {
                res[i] = bytes[i];
                res[i] ^= keyBytes[i % keyLength];
            }

            return res;
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="bytes">需要解密的字节码</param>
        /// <returns></returns>
        public static byte[] Decrypt(string key, byte[] bytes)
        {
            if (string.IsNullOrEmpty(key))
            {
                return bytes;
            }
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            int keyLength = keyBytes.Length;

            int length = bytes.Length;
            byte[] res = new byte[length];
            for (int i = 0; i < length; i++)
            {
                res[i] = bytes[i];
                res[i] ^= keyBytes[i % keyLength];
            }

            return res;
        }


        public static string EncryptToString(string key, byte[] bytes)
        {
            byte[] res = Encrypt(key, bytes);
            return Convert.ToBase64String(res);
        }

        public static byte[] DecryptFromString(string key, string content)
        {
            byte[] bytes = Convert.FromBase64String(content);
            return Decrypt(key, bytes);
        }
    }
}
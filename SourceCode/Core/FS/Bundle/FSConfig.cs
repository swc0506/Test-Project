using UnityEngine;

namespace Core.FS
{
    public class FSConfig : ScriptableObject
    {
        /// <summary>
        /// AssetBundle 读取偏移
        /// </summary>
        [SerializeField] private int offset = 0;

        /// <summary>
        /// 映射文件标识
        /// </summary>
        [SerializeField] private string flag = null;

        /// <summary>
        /// 加密密钥
        /// </summary>
        [SerializeField] private string encryptKey = null;

        public int Offset
        {
            get { return offset; }
        }

        public string Flag
        {
            get { return flag; }
        }

        public string EncryptKey
        {
            get { return encryptKey; }
        }


        private static FSConfig instance;

        public static FSConfig Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = Resources.Load<FSConfig>("FS/Key");
                }

                return instance;
            }
        }
    }
}
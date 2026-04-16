namespace Core.Storage
{
    public interface IStorable
    {
        /// <summary>
        /// 设置int数值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="value">对应的值</param>
        void SetInt(string key, int value);

        /// <summary>
        /// 设置float数值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="value">对应的值</param>
        void SetFloat(string key, float value);

        /// <summary>
        /// 设置string数值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="value">对应的值</param>
        void SetString(string key, string value);

        /// <summary>
        /// 设置bool数值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="value">对应的值</param>
        void SetBool(string key, bool value);

        /// <summary>
        /// 设置对象数值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        void SetObject<T>(string key, T value);


        /// <summary>
        /// 获取int类型的值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>存储的值</returns>
        int GetInt(string key, int defValue);

        /// <summary>
        /// 获取float类型的值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>存储的值</returns>
        float GetFloat(string key, float defValue);

        /// <summary>
        /// 获取string类型的值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>存储的值</returns>
        string GetString(string key, string defValue);

        /// <summary>
        /// 获取bool类型的值
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <param name="defValue">默认值</param>
        /// <returns>存储的值</returns>
        bool GetBool(string key, bool defValue);

        /// <summary>
        /// 获取object类型的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        T GetObject<T>(string key, T defValue);

        /// <summary>
        /// 是否已存在key
        /// </summary>
        /// <param name="key">存储的key</param>
        /// <returns>结果</returns>
        bool HasKey(string key);

        /// <summary>
        /// 删除对应存储值
        /// </summary>
        /// <param name="key">存储的key</param>
        void DeleteKey(string key);

        /// <summary>
        /// 删除所有的存储值
        /// </summary>
        void DeleteAll();
    }
}
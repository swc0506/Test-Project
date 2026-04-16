using System.Collections.Generic;
using Core.Net;
using UnityEngine;

namespace Core
{
    public class HttpUtils
    {
        private const int TIMEOUT = 8;

        #region Get

        public static void Get(string url, HttpCompletedAction callback, int timeout = TIMEOUT,
            Dictionary<string, string> headers = null)
        {
            HttpGetTask requestTask = new HttpGetTask(url);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request();
        }

        public static void Get(string url, HttpCompletedAction callback, int timeout)
        {
            Get(url, callback, timeout, null);
        }

        public static void Get(string url, HttpCompletedAction callback, Dictionary<string, string> headers)
        {
            Get(url, callback, TIMEOUT, headers);
        }

        #endregion

        #region ChinaGet

        public static void ChinaGet(IEnumerable<string> urls, HttpCompletedAction callback, int timeout = TIMEOUT,
            Dictionary<string, string> headers = null)
        {
            ChainGetTask requestTask = new ChainGetTask(urls);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request();
        }

        public static void ChinaGet(IEnumerable<string> urls, HttpCompletedAction callback, int timeout)
        {
            ChinaGet(urls, callback, timeout, null);
        }

        public static void ChinaGet(IEnumerable<string> urls, HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            ChinaGet(urls, callback, TIMEOUT, headers);
        }

        #endregion


        #region Post text

        public static void Post(string url, string text, HttpCompletedAction callback, int timeout = TIMEOUT,
            Dictionary<string, string> headers = null)
        {
            HttpPostTask requestTask = new HttpPostTask(url);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(text);
        }

        public static void Post(string url, string text, HttpCompletedAction callback, int timeout)
        {
            Post(url, text, callback, timeout, null);
        }

        public static void Post(string url, string text, HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            Post(url, text, callback, TIMEOUT, headers);
        }

        #endregion


        #region ChinaPost text

        public static void ChinaPost(IEnumerable<string> urls, string text, HttpCompletedAction callback,
            int timeout = TIMEOUT, Dictionary<string, string> headers = null)
        {
            ChainPostTask requestTask = new ChainPostTask(urls);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(text);
        }

        public static void ChinaPost(IEnumerable<string> urls, string text, HttpCompletedAction callback, int timeout)
        {
            ChinaPost(urls, text, callback, timeout, null);
        }

        public static void ChinaPost(IEnumerable<string> urls, string text, HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            ChinaPost(urls, text, callback, TIMEOUT, headers);
        }

        #endregion


        #region Post map

        public static void Post(string url, Dictionary<string, string> map, HttpCompletedAction callback,
            int timeout = TIMEOUT, Dictionary<string, string> headers = null)
        {
            HttpPostTask requestTask = new HttpPostTask(url);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(map);
        }

        public static void Post(string url, Dictionary<string, string> map, HttpCompletedAction callback, int timeout)
        {
            Post(url, map, callback, timeout, null);
        }

        public static void Post(string url, Dictionary<string, string> map, HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            Post(url, map, callback, TIMEOUT, headers);
        }

        #endregion


        #region ChinaPost text

        public static void ChinaPost(IEnumerable<string> urls, Dictionary<string, string> map,
            HttpCompletedAction callback, int timeout = TIMEOUT, Dictionary<string, string> headers = null)
        {
            ChainPostTask requestTask = new ChainPostTask(urls);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(map);
        }

        public static void ChinaPost(IEnumerable<string> urls, Dictionary<string, string> map,
            HttpCompletedAction callback, int timeout)
        {
            ChinaPost(urls, map, callback, timeout, null);
        }

        public static void ChinaPost(IEnumerable<string> urls, Dictionary<string, string> map,
            HttpCompletedAction callback, Dictionary<string, string> headers)
        {
            ChinaPost(urls, map, callback, TIMEOUT, headers);
        }

        #endregion

        #region Post form

        public static void Post(string url, WWWForm form, HttpCompletedAction callback,
            int timeout = TIMEOUT, Dictionary<string, string> headers = null)
        {
            HttpPostTask requestTask = new HttpPostTask(url);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(form);
        }

        public static void Post(string url, WWWForm form, HttpCompletedAction callback, int timeout)
        {
            Post(url, form, callback, timeout, null);
        }

        public static void Post(string url, WWWForm form, HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            Post(url, form, callback, TIMEOUT, headers);
        }

        #endregion

        #region ChinaPost form

        public static void ChinaPost(IEnumerable<string> urls, WWWForm form, HttpCompletedAction callback,
            int timeout = TIMEOUT, Dictionary<string, string> headers = null)
        {
            ChainPostTask requestTask = new ChainPostTask(urls);
            requestTask.CompletedEvent += callback;
            requestTask.timeout = timeout;
            requestTask.SetRequestHeaders(headers);
            requestTask.Request(form);
        }

        public static void ChinaPost(IEnumerable<string> urls, WWWForm form,
            HttpCompletedAction callback, int timeout)
        {
            ChinaPost(urls, form, callback, timeout, null);
        }

        public static void ChinaPost(IEnumerable<string> urls, WWWForm form,
            HttpCompletedAction callback,
            Dictionary<string, string> headers)
        {
            ChinaPost(urls, form, callback, TIMEOUT, headers);
        }

        #endregion
    }
}
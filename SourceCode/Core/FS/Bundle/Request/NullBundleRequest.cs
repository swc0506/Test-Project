using System.Collections;
using UnityEngine;

namespace Core.FS
{
    public class NullBundleRequest : AbstractBundleRequest
    {
        private static WaitForEndOfFrame endOfFrame;

        public override void Request()
        {
            if (null == endOfFrame)
            {
                endOfFrame = new WaitForEndOfFrame();
            }

            MonoEventProxy.Instance.StartCoroutine(RequestCompleted());
        }

        private IEnumerator RequestCompleted()
        {
            yield return endOfFrame;
            OnLoadCompleted(null);
        }
    }
}
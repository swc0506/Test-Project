using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZM.AssetFrameWork;
public class LoadSceneManager : MonoSingleton<LoadSceneManager>
{
    public void LoadSceneAsync(string sceneName,Action LoadSceneFinish)
    {
        UIModule.Instance.PopUpWindow<LoadingWindow>();
        StartCoroutine(AsyncLoadScene(sceneName, LoadSceneFinish));
    }
    IEnumerator AsyncLoadScene(string sceneName, Action LoadSceneFinish)
    {
        //异步加载场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        //默认不允许场景激活
        operation.allowSceneActivation = false;

        //当前加载进度
        float curProgress = 0;
        //最大加载进度
        float maxProgress = 100;
        //Unity场景加载进度只会从0-0.9  ，剩余0.1需要我们使用代码做一个过度
        while (curProgress < 90)
        {
            curProgress = operation.progress * 100.0f;
            //通过一个事件把当前进度抛出，
            UIEventControl.DispensEvent(UIEventEnum.SceneProgressUpdate, curProgress);
            yield return null;
        }

        while (curProgress < maxProgress)
        {
            curProgress++;
            UIEventControl.DispensEvent(UIEventEnum.SceneProgressUpdate, curProgress);
            //等一个空帧是为了让UI有渲染的过程
            yield return null;
        }
        //激活已加载完成的场景
        operation.allowSceneActivation = true;
        yield return null;
        LoadSceneFinish?.Invoke();
     
        
    }
}

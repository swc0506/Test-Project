using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ZM.ZMAsset;

/// <summary>
/// 音效播放器(优先级)
/// </summary>
public class AudioController : MonoBehaviour {
    /// <summary>
    /// 静态单利
    /// </summary>
    private static AudioController _instance;
    /// <summary>
    ///  已经加载过声音的资源池
    /// </summary>
    private Dictionary<string, AudioClip> soundAudioDic = new Dictionary<string, AudioClip>();
    /// <summary>
    /// 带优先级的音源对象池
    /// </summary>
    private List<AudioSourceInfo> mAudioInfoList = new List<AudioSourceInfo>();
    /// <summary>
    /// 音乐音源
    /// </summary>
    private static AudioSource mMusicSource;
    /// <summary>
    /// 最大缓存数量
    /// </summary>
    private static int MaxCacheCount=30;
    /// <summary>
    /// 音效音量
    /// </summary>
    private float mSoundVolume = 0.3f;
    /// <summary>
    /// 音乐音量
    /// </summary>
    private float mMusicVolume = 0.3f;

    /// <summary>
    /// 单利
    /// </summary>
    /// <returns></returns>
    public static AudioController GetInstance()
    {
        if(_instance==null)
        {
            //首先创建一个空物体
            GameObject gb = new GameObject();
            gb.name = "AudioManager";
            //动态添加脚本
            _instance = gb.AddComponent<AudioController>();
            //加载音乐音源对象
            mMusicSource = gb.AddComponent<AudioSource>();
            //加载10个音源对象
            for (int i = 0; i < MaxCacheCount; i++)
            {
                AudioSource audioSource = gb.AddComponent<AudioSource>();
                AudioSourceInfo asInfo = new AudioSourceInfo();
                asInfo.audioSource = audioSource;
              _instance.mAudioInfoList.Add(asInfo);
            }
            //物体不要销毁
            DontDestroyOnLoad(gb);
        }
        //最后返回单例
        return _instance;
    }

    /// <summary>
    /// 获取音频剪辑
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private AudioClip GetAudioClip(string fullPath)
    {
        AudioClip clip=null;
        //判断资源池当中有没有这个声音
        if (!soundAudioDic.TryGetValue(fullPath, out clip) &&clip ==null)
        {
            //clip = Resources.Load<AudioClip>("Audio/" + name);//同一个声音反复加载，能不能存起来，不要加载
            clip = ZMAsset.LoadAudio(fullPath);//同一个声音反复加载，能不能存起来，不要加载
            //放入字典
            soundAudioDic.Add(fullPath, clip);
            //soundAudioDic.Clear();
        }
        return clip;
    }
    /// <summary>
    /// 获取优先级最小的音源信息
    /// </summary>
    /// <returns></returns>
    private AudioSourceInfo GetMinPriorityAudioInfo()
    {
        //首先找一下有没有闲着的。如果有不用考虑优先级，播放
        //如果没有，那么需要找一个优先级最低的 ,首先停掉正在播的，然后继续播其他的声音
        int minPriority = 1000;//最小优先级

        AudioSourceInfo audioSourceInfo = null;

        for (int i = 0; i < mAudioInfoList.Count; i++)
        {
            if (mAudioInfoList[i].audioSource.isPlaying == false)
            {
                audioSourceInfo = mAudioInfoList[i];
                mAudioInfoList[i].priority = -1;
                break;//找到空闲的，停下来
            }
            else
            {
                //判断当前优先级是否大于我的记录
                if (mAudioInfoList[i].priority < minPriority)
                {
                    //当前的优先级最小，记录更小
                    minPriority = mAudioInfoList[i].priority;
                    audioSourceInfo = mAudioInfoList[i];
                }
            }
        }
        return audioSourceInfo;
    }
    /// <summary>
    /// 播放一个音效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="priority"></param>
    public void PlaySoundByPath(string fullPath,int priority, float soundVolume = -1)
    {
        //获取需要播放的音源
        AudioClip clip=GetAudioClip(fullPath);

        //获取空闲或优先级最小的音频信息
        AudioSourceInfo audioSourceInfo = GetMinPriorityAudioInfo();

        //判断找到的最小的优先级是否大于需要播放的优先级
        if (audioSourceInfo.priority<priority)
        {
            //切换声音
            audioSourceInfo.audioSource.Stop();
            audioSourceInfo.audioSource.clip = clip;
            audioSourceInfo.audioSource.volume = soundVolume > 0 ? soundVolume : mSoundVolume; ;
            audioSourceInfo.audioSource.Play();
            //改变优先级 
            audioSourceInfo.priority = priority;
        }
        else
        {
            Debug.Log("name："+ name +" 音频优先级过低且音源池无空闲音频，无法正常播放！");
        }
    }
    public void PlaySoundByAudioClip(AudioClip audioClip,bool isLoop, int priority,float soundVolume=-1)
    {
        //获取空闲或优先级最小的音频信息
        AudioSourceInfo audioSourceInfo = GetMinPriorityAudioInfo();

        //判断找到的最小的优先级是否大于需要播放的优先级
        if (audioSourceInfo.priority < priority)
        {
            //切换声音
            audioSourceInfo.audioSource.Stop();
            audioSourceInfo.audioSource.clip = audioClip;
            audioSourceInfo.audioSource.volume = soundVolume>0?soundVolume: mSoundVolume;
            audioSourceInfo.audioSource.loop = isLoop;
            audioSourceInfo.audioSource.Play();
            
            //改变优先级 
            audioSourceInfo.priority = priority;
        }
        else
        {
            Debug.Log("name：" + name + " 音频优先级过低且音源池无空闲音频，无法正常播放！");
        }
    }
    public void StopSound(AudioClip audioClip)
    {
        foreach (var item in mAudioInfoList)
        {
            if (item.audioSource.clip==audioClip)
            {
                item.audioSource.loop = false;
                item.audioSource.Stop();
                item.audioSource.clip=null ;
                item.priority = 0;
            }
        }
    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayMusic(string fullPath)
    {
      AudioClip clip= GetAudioClip(name);
        if (clip!=null)
        {
            mMusicSource.Stop();
            mMusicSource.clip = clip;
            mMusicSource.volume = mMusicVolume;
            mMusicSource.Play();
        }
    }
    /// <summary>
    /// 渐隐播放背景音乐 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="duration"></param>
    public void PlayMusicFade(string fullPath, float duration)
    {
        AudioClip clip = GetAudioClip(fullPath);
        if (clip != null)
        {
            mMusicSource.Stop();
            mMusicSource.loop = true;
            mMusicSource.clip = clip;
            mMusicSource.volume = 0;
            DG.Tweening.DOTween.To(()=> mMusicSource.volume, x=> mMusicSource.volume = x, mMusicVolume, duration);
            mMusicSource.Play();
        }
    }
}
/// <summary>
/// 音源信息
/// </summary>
public  class AudioSourceInfo
{
    /// <summary>
    /// 播放声音的音源
    /// </summary>
    public AudioSource audioSource;
    /// <summary>
    /// 优先级  -1》音源处于闲置状态，
    /// </summary>
    public int priority=-1;
}

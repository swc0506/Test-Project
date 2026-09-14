using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Audio;
using ZM.ZMAsset;

public enum AudioWorldType
{
    None,
    HallWorld,
    BattleWorld,
}

/// <summary>
/// 音效播放器(优先级)
/// </summary>
public class AudioController : MonoBehaviour
{
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
    private static AudioSource mHallMusicSource;

    private static AudioSource mBattleMusicSource;

    /// <summary>
    /// 最大缓存数量
    /// </summary>
    private static int MaxCacheCount = 10;

    /// <summary>
    /// 音效音量
    /// </summary>
    private float mSoundVolume = 0.6f;

    /// <summary>
    /// 音乐音量
    /// </summary>
    private float mMusicVolume = 0.5f;

    private AudioMixer mGameAudioMixer;

    /// <summary>
    /// 所有音效主节点
    /// </summary>
    private AudioMixerGroup mAudioMixerMaster;

    /// <summary>
    /// 大厅音效组
    /// </summary>
    private AudioMixerGroup mAudioMixerHallGroup;

    /// <summary>
    /// 战斗音效组
    /// </summary>
    private AudioMixerGroup mAudioMixerBattleGroup;

    /// <summary>
    /// 单利
    /// </summary>
    /// <returns></returns>
    public static AudioController GetInstance()
    {
        if (_instance == null)
        {
            //首先创建一个空物体
            GameObject gb = new GameObject();
            gb.name = "AudioManager";
            //动态添加脚本
            _instance = gb.AddComponent<AudioController>();
            //加载音乐音源对象
            mHallMusicSource = gb.AddComponent<AudioSource>();
            mBattleMusicSource = gb.AddComponent<AudioSource>();
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

    private async void Awake()
    {
        //加载和获取Unity混音器系统
        mGameAudioMixer =
            await ZMAsset.LoadResourceAsync<AudioMixer>(AssetsPathConfig.HALL_SOUNDS_PATH + "GameAudioMixer.mixer");
        mAudioMixerMaster = mGameAudioMixer.FindMatchingGroups("Master")[0];
        mAudioMixerBattleGroup = mGameAudioMixer.FindMatchingGroups("Battle")[0];
        mAudioMixerHallGroup = mGameAudioMixer.FindMatchingGroups("Hall")[0];
        UIEventControl.AddEvent(UIEventEnum.SwitchOutBattle, SwitchOutBattle);
        UIEventControl.AddEvent(UIEventEnum.SwitchInBattle, SwitchInBattle);
    }

    private void SwitchOutBattle(object data)
    {
        //混音器音量大小-80->20
        //回到大厅 
        // float targetVolume = -80.0f;
        // float curVolume = 0;
        // DG.Tweening.DOTween.To(()=> curVolume, x=>curVolume = x, targetVolume, 0.5f).OnUpdate(() =>
        // {
        //     mGameAudioMixer.SetFloat("Battle",curVolume);
        // });
        mGameAudioMixer.SetFloat("Battle", -80.0f);
        mGameAudioMixer.SetFloat("Hall", 0);
    }

    private void SwitchInBattle(object data)
    {
        // float targetVolume = 0f;
        // float curVolume = -80;
        // DG.Tweening.DOTween.To(()=> curVolume, x=>curVolume = x, targetVolume, 0.5f).OnUpdate(() =>
        // {
        //     mGameAudioMixer.SetFloat("Battle",curVolume);
        // });
        mGameAudioMixer.SetFloat("Battle", 0);
        mGameAudioMixer.SetFloat("Hall", -80.0f);
    }

    /// <summary>   
    /// 获取音频剪辑
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private AudioClip GetAudioClip(string fullPath)
    {
        AudioClip clip = null;
        //判断资源池当中有没有这个声音
        if (!soundAudioDic.TryGetValue(fullPath, out clip) && clip == null)
        {
            //clip = Resources.Load<AudioClip>("Audio/" + name);//同一个声音反复加载，能不能存起来，不要加载
            clip = ZMAsset.LoadAudio(fullPath); //同一个声音反复加载，能不能存起来，不要加载
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
        int minPriority = 1000; //最小优先级

        AudioSourceInfo audioSourceInfo = null;

        for (int i = 0; i < mAudioInfoList.Count; i++)
        {
            if (mAudioInfoList[i].audioSource.isPlaying == false)
            {
                audioSourceInfo = mAudioInfoList[i];
                mAudioInfoList[i].priority = -1;
                break; //找到空闲的，停下来
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
    public void PlaySoundByPath(string fullPath, int priority, float soundVolume = -1,
        AudioWorldType audioWorldType = AudioWorldType.BattleWorld)
    {
        //获取需要播放的音源
        AudioClip clip = GetAudioClip(fullPath);

        //获取空闲或优先级最小的音频信息
        AudioSourceInfo audioSourceInfo = GetMinPriorityAudioInfo();

        //判断找到的最小的优先级是否大于需要播放的优先级
        if (audioSourceInfo.priority < priority)
        {
            //切换声音
            audioSourceInfo.audioSource.outputAudioMixerGroup = audioWorldType == AudioWorldType.BattleWorld
                ? mAudioMixerBattleGroup
                : mAudioMixerHallGroup;
            audioSourceInfo.audioSource.Stop();
            audioSourceInfo.audioSource.clip = clip;
            audioSourceInfo.audioSource.volume = soundVolume > 0 ? soundVolume : mSoundVolume;
            ;
            audioSourceInfo.audioSource.Play();
            //改变优先级 
            audioSourceInfo.priority = priority;
        }
        else
        {
            Debug.Log("name：" + name + " 音频优先级过低且音源池无空闲音频，无法正常播放！");
        }
    }

    public void PlaySoundByAudioClip(AudioClip audioClip, bool isLoop, int priority, float soundVolume = -1,
        AudioWorldType audioWorldType = AudioWorldType.BattleWorld)
    {
        //获取空闲或优先级最小的音频信息
        AudioSourceInfo audioSourceInfo = GetMinPriorityAudioInfo();

        //判断找到的最小的优先级是否大于需要播放的优先级
        if (audioSourceInfo.priority < priority)
        {
            //切换声音
            audioSourceInfo.audioSource.Stop();
            audioSourceInfo.audioSource.clip = audioClip;
            audioSourceInfo.audioSource.outputAudioMixerGroup = audioWorldType == AudioWorldType.BattleWorld
                ? mAudioMixerBattleGroup
                : mAudioMixerHallGroup;
            audioSourceInfo.audioSource.volume = soundVolume > 0 ? soundVolume : mSoundVolume;
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
            if (item.audioSource.clip == audioClip)
            {
                item.audioSource.loop = false;
                item.audioSource.Stop();
                item.audioSource.clip = null;
                item.priority = 0;
            }
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayMusic(string fullPath, AudioWorldType audioWorldType = AudioWorldType.BattleWorld)
    {
        AudioClip clip = GetAudioClip(name);
        if (clip != null)
        {
            var mMusicSource = audioWorldType == AudioWorldType.HallWorld ? mHallMusicSource : mBattleMusicSource;
            mMusicSource.Stop();
            mMusicSource.outputAudioMixerGroup = audioWorldType == AudioWorldType.BattleWorld
                ? mAudioMixerBattleGroup
                : mAudioMixerHallGroup;
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
    public void PlayMusicFade(string fullPath, float duration,
        AudioWorldType audioWorldType = AudioWorldType.BattleWorld)
    {
        AudioClip clip = GetAudioClip(fullPath);
        if (clip != null)
        {
            var mMusicSource = audioWorldType == AudioWorldType.HallWorld ? mHallMusicSource : mBattleMusicSource;
            mMusicSource.Stop();
            mMusicSource.loop = true;
            mMusicSource.outputAudioMixerGroup = audioWorldType == AudioWorldType.BattleWorld
                ? mAudioMixerBattleGroup
                : mAudioMixerHallGroup;
            mMusicSource.clip = clip;
            mMusicSource.volume = 0;
            DG.Tweening.DOTween.To(() => mMusicSource.volume, x => mMusicSource.volume = x, mMusicVolume, duration);
            mMusicSource.Play();
        }
    }

    private void OnDestroy()
    {
        UIEventControl.RemoveEvent(UIEventEnum.SwitchOutBattle, SwitchOutBattle);
        UIEventControl.RemoveEvent(UIEventEnum.SwitchInBattle, SwitchInBattle);
    }
}

/// <summary>
/// 音源信息
/// </summary>
public class AudioSourceInfo
{
    /// <summary>
    /// 播放声音的音源
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// 优先级  -1》音源处于闲置状态，
    /// </summary>
    public int priority = -1;
}
using System.Collections.Generic;
using Core.FS;
using UnityEngine;

namespace Core.Sound
{
    class AsyncPlayInfo : IClearable
    {
        public int id;
        public AssetAsyncHandler handler;
        public string groupName;
        public int priority;
        public PlaySoundParams playParams;

        public void Clear()
        {
            id = 0;
            groupName = null;
            priority = 0;
        }
    }

    public class SoundManager : Singleton<SoundManager>
    {
        private const int MAX_SOUND_COUNT = 20;
        private const int DEFAULT_PRIORITY = 128;

        private static PlaySoundParams DefalutPlayParams = PlaySoundParams.Create();

        private Transform soundRoot;
        private Dictionary<string, SoundGroup> groupMap;

        private AtomicInt atomicId;

        //默认同步加载
        public static bool isAsync = true;
        private AssetPackage assetPkg;
        private Dictionary<string, List<AsyncPlayInfo>> asyncPlayMap;

        protected override void OnInitial()
        {
            base.OnInitial();
            GameObject soundGo = new GameObject("Sound");
            GameObject.DontDestroyOnLoad(soundGo);
            soundRoot = soundGo.transform;
            AudioListener audioListener = GameObject.FindObjectOfType<AudioListener>();
            if (null == audioListener)
            {
                soundGo.AddComponent<AudioListener>();
            }

            groupMap = new Dictionary<string, SoundGroup>();
            atomicId = new AtomicInt();
            asyncPlayMap = new Dictionary<string, List<AsyncPlayInfo>>();
        }

        public void SetAssetPackage(AssetPackage assetPkg)
        {
            this.assetPkg = assetPkg;
        }

        public void SetAssetPackageName(string pkgName)
        {
            this.assetPkg = ResourceManager.Instance.Get(pkgName);
        }

        private SoundGroup InternalAddGroup(string groupName, int maxSoundCount)
        {
            SoundGroup group = new SoundGroup(groupName, maxSoundCount, soundRoot.transform);
            groupMap.Add(groupName, group);
            return group;
        }

        public bool AddSoundGroup(string groupName, int maxSoundCount)
        {
            if (!string.IsNullOrEmpty(groupName) && !groupMap.ContainsKey(groupName))
            {
                InternalAddGroup(groupName, maxSoundCount);
                return true;
            }

            return false;
        }

        public bool AddSoundGroup(string groupName)
        {
            return AddSoundGroup(groupName, MAX_SOUND_COUNT);
        }

        public SoundGroup GetSoundGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName) && groupMap.TryGetValue(groupName, out var group))
            {
                return group;
            }

            return null;
        }

        public bool ContainsSoundGroup(string groupName)
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                return groupMap.ContainsKey(groupName);
            }

            return false;
        }

        public bool SetGroupMute(string groupName, bool mute)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null != group)
            {
                group.Mute = mute;
                return true;
            }

            return false;
        }

        public bool SetGroupVolume(string groupName, float volume)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null != group)
            {
                group.Volume = volume;
                return true;
            }

            return false;
        }

        public bool PauseGroup(string groupName)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null != group)
            {
                group.PauseAllSounds();
                return true;
            }

            return false;
        }

        public bool ResumeGroup(string groupName)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null != group)
            {
                group.ResumeAllSounds();
                return true;
            }

            return false;
        }

        public bool StopGroup(string groupName)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null != group)
            {
                group.StopAllSounds();
                return true;
            }

            return false;
        }

        public int Play(string path, string groupName, int priority, PlaySoundParams playParams)
        {
            int id = atomicId.GetAndIncrement();
            if (isAsync)
            {
                AssetAsyncHandler asyncHandler = assetPkg.LoadAsync(path, priority);
                asyncHandler.CompletedEvent += OnLoadAudioCompleted;
                if (!asyncPlayMap.TryGetValue(path, out var list))
                {
                    list = new List<AsyncPlayInfo>();
                    asyncPlayMap.Add(path, list);
                }

                AsyncPlayInfo asyncPlay = ReferencePool.Global.Pop<AsyncPlayInfo>();
                asyncPlay.id = id;
                asyncPlay.handler = asyncHandler;
                asyncPlay.groupName = groupName;
                asyncPlay.priority = priority;
                asyncPlay.playParams = playParams;
                list.Add(asyncPlay);
            }
            else
            {
                var asset = assetPkg.Load(path);
                OnPlayAudio(asset, path, groupName, id, priority, playParams);
            }

            return id;
        }

        public int Play(string path, string groupName, int priority)
        {
            return Play(path, groupName, priority, DefalutPlayParams);
        }

        public int Play(string path, string groupName, PlaySoundParams playParams)
        {
            return Play(path, groupName, DEFAULT_PRIORITY, playParams);
        }

        public int Play(string path, string groupName)
        {
            return Play(path, groupName, DEFAULT_PRIORITY, DefalutPlayParams);
        }

        public int Play(string path, string groupName, float pitch)
        {
            PlaySoundParams playParams = PlaySoundParams.Create();
            playParams.pitch = pitch;
            return Play(path, groupName, playParams);
        }

        public int Play(string path, string groupName, float time, bool loop, float volume, float fadeInTime,
            float fadeOutTime)
        {
            PlaySoundParams playParams = PlaySoundParams.Create();
            playParams.time = time;
            playParams.loop = loop;
            playParams.volume = volume;
            playParams.fadeInTime = fadeInTime;
            playParams.finishedFadeOutTime = fadeOutTime;
            return Play(path, groupName, playParams);
        }

        public int Play(string path, string groupName, bool loop, float volume)
        {
            PlaySoundParams playParams = PlaySoundParams.Create();
            playParams.loop = loop;
            playParams.volume = volume;
            return Play(path, groupName, playParams);
        }

        public int Play(string path, string groupName, bool loop)
        {
            PlaySoundParams playParams = PlaySoundParams.Create();
            playParams.loop = loop;
            return Play(path, groupName, playParams);
        }
        
        private bool IsStopInLoading(int id)
        {
            foreach (var item in asyncPlayMap)
            {
                int count = item.Value.Count;
                for (int i = 0; i < count; i++)
                {
                    var playInfo = item.Value[i];
                    if (playInfo.id == id)
                    {
                        playInfo.handler.CompletedEvent -= OnLoadAudioCompleted;
                        ReferencePool.Global.Push(playInfo);
                        item.Value.RemoveAt(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Pause(int id, float fadeOutTime)
        {
            foreach (var item in groupMap)
            {
                if (item.Value.PauseSound(id, fadeOutTime))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Pause(int id)
        {
            return Pause(id, 0);
        }

        public bool Resume(int id, float fadeInTime)
        {
            foreach (var item in groupMap)
            {
                if (item.Value.ResumeSound(id, fadeInTime))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Resume(int id)
        {
            return Resume(id, 0);
        }

        public bool Stop(int id, float fadeOutTime)
        {
            if (!IsStopInLoading(id))
            {
                foreach (var item in groupMap)
                {
                    if (item.Value.StopSound(id, fadeOutTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Stop(int id)
        {
            return Stop(id, 0);
        }

        private void OnLoadAudioCompleted(AssetObject asset, string path)
        {
            if (asyncPlayMap.TryGetValue(path, out var list))
            {
                foreach (var item in list)
                {
                    OnPlayAudio(asset, path, item.groupName, item.id, item.priority, item.playParams);
                    ReferencePool.Global.Push(item);
                }

                asyncPlayMap.Remove(path);
            }
        }

        private void OnPlayAudio(AssetObject asset, string path, string groupName, int id, int priority,
            PlaySoundParams playParams)
        {
            if (null != asset)
            {
                AudioClip clip = asset.Get<AudioClip>();
                if (null != clip)
                {
                    InternalPlaySound(groupName, id, clip, priority, playParams);
                }
                else
                {
                    Logger.DebugFormat("AudioClip Is Null:{0}", path);
                }
            }
        }

        private void InternalPlaySound(string groupName, int id, AudioClip clip, int priority,
            PlaySoundParams playParams)
        {
            SoundGroup group = GetSoundGroup(groupName);
            if (null == group)
            {
                group = InternalAddGroup(groupName, MAX_SOUND_COUNT);
            }

            group.PlaySound(id, clip, priority, playParams);
        }

        public void Update(float deltaTime)
        {
            foreach (var item in groupMap)
            {
                item.Value.Update(deltaTime);
            }
        }

        public void StopAll()
        {
            foreach (var item in groupMap)
            {
                item.Value.StopAllSounds();
            }
        }
    }
}
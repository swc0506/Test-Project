using UnityEngine;

namespace Core.Sound
{
    public class SoundPlayer : IUpdateable, IClearable
    {
        enum SoundState
        {
            Stop,
            Playing,
            Pause
        }

        private SoundGroup context;
        private Transform trans;
        private AudioSource audioSource;

        public int Id { get; private set; }
        public float FinishedFadeOutTime { get; private set; }
        public float StartPlayTime { get; private set; }

        private SoundState state;
        private bool mute = false;
        private float volume = 1.0f;
        private float volumeWhenPause;

        private float startVolume;
        private float targetVolume;
        private float duration;
        private float elapseTime;

        public SoundPlayer(SoundGroup context, GameObject go)
        {
            this.context = context;
            trans = go.transform;
            audioSource = go.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.rolloffMode = AudioRolloffMode.Custom;
        }

        /// <summary>
        /// 获取当前是否正在播放。
        /// </summary>
        public bool IsPlaying
        {
            get { return audioSource.isPlaying; }
        }

        /// <summary>
        /// 获取声音长度。
        /// </summary>
        public float Length
        {
            get { return audioSource.clip.length; }
        }

        /// <summary>
        /// 获取或设置播放位置。
        /// </summary>
        public float Time
        {
            get { return audioSource.time; }
            set { audioSource.time = value; }
        }

        /// <summary>
        /// 获取或设置是否静音。
        /// </summary>
        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                RefreshMute();
            }
        }

        /// <summary>
        /// 获取或设置是否循环播放。
        /// </summary>
        public bool Loop
        {
            get { return audioSource.loop; }
            set { audioSource.loop = value; }
        }

        /// <summary>
        /// 获取或设置声音优先级。 越小越紧急
        /// </summary>
        public int Priority
        {
            get { return audioSource.priority; }
            set { audioSource.priority = value; }
        }


        /// <summary>
        /// 获取或设置音量大小。
        /// </summary>
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                RefreshVolume();
            }
        }

        /// <summary>
        /// 获取或设置声音音调。
        /// </summary>
        public float Pitch
        {
            get { return audioSource.pitch; }
            set { audioSource.pitch = value; }
        }

        /// <summary>
        /// 获取或设置声音立体声声相。
        /// </summary>
        public float PanStereo
        {
            get { return audioSource.panStereo; }
            set { audioSource.panStereo = value; }
        }

        /// <summary>
        /// 获取或设置声音空间混合量。
        /// </summary>
        public float SpatialBlend
        {
            get { return audioSource.spatialBlend; }
            set { audioSource.spatialBlend = value; }
        }

        /// <summary>
        /// 混响区域混合。
        /// </summary>
        public float ReverbZoneMix
        {
            get { return audioSource.reverbZoneMix; }
            set { audioSource.reverbZoneMix = value; }
        }

        /// <summary>
        /// 获取或设置声音多普勒等级。
        /// </summary>
        public float DopplerLevel
        {
            get { return audioSource.dopplerLevel; }
            set { audioSource.dopplerLevel = value; }
        }

        /// <summary>
        /// 获取或设置声音最大距离。
        /// </summary>
        public float MaxDistance
        {
            get { return audioSource.maxDistance; }

            set { audioSource.maxDistance = value; }
        }

        /// <summary>
        /// 获取或设置声音最大距离。
        /// </summary>
        public float MinDistance
        {
            get { return audioSource.minDistance; }

            set { audioSource.minDistance = value; }
        }


        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="fadeInTime">声音淡入时间 秒为单位</param>
        public void Play(float fadeInTime)
        {
            if (state == SoundState.Playing)
            {
                return;
            }

            state = SoundState.Playing;
            audioSource.Play();
            if (fadeInTime > 0)
            {
                float volume = audioSource.volume;
                audioSource.volume = 0;
                SmoothVolume(volume, fadeInTime);
            }
        }

        /// <summary>
        /// 停止播放声音
        /// </summary>
        /// <param name="fadeOutTime">声音淡出时间 秒为单位</param>
        public void Stop(float fadeOutTime)
        {
            if (state == SoundState.Stop)
            {
                return;
            }

            state = SoundState.Stop;
            if (fadeOutTime > 0)
            {
                SmoothVolume(0, fadeOutTime);
            }
            else
            {
                audioSource.Stop();
            }
        }

        /// <summary>
        /// 暂停播放声音
        /// </summary>
        /// <param name="fadeOutTime">声音淡出时间 秒为单位</param>
        public void Pause(float fadeOutTime)
        {
            if (state == SoundState.Playing)
            {
                state = SoundState.Pause;
                if (fadeOutTime > 0)
                {
                    volumeWhenPause = audioSource.volume;
                    SmoothVolume(0, fadeOutTime);
                }
                else
                {
                    audioSource.Pause();
                }
            }
        }

        /// <summary>
        /// 恢复播放声音
        /// </summary>
        /// <param name="fadeInTime">声音淡入时间 秒为单位</param>
        public void Resume(float fadeInTime)
        {
            if (state == SoundState.Pause)
            {
                state = SoundState.Playing;
                audioSource.UnPause();
                if (fadeInTime > 0)
                {
                    SmoothVolume(volumeWhenPause, fadeInTime);
                }
                else
                {
                    audioSource.volume = volumeWhenPause;
                }
            }
        }

        public void SetWorldPosition(Vector3 pos)
        {
            trans.position = pos;
        }

        internal void SetData(AudioClip clip, int id, float finishedFadeOutTime)
        {
            audioSource.clip = clip;
            Id = id;
            FinishedFadeOutTime = finishedFadeOutTime;
            StartPlayTime = UnityEngine.Time.unscaledTime;
        }

        internal void RefreshMute()
        {
            audioSource.mute = context.Mute || mute;
        }

        internal void RefreshVolume()
        {
            audioSource.volume = context.Volume * volume;
            if (state == SoundState.Pause)
            {
                volumeWhenPause = audioSource.volume;
            }
        }

        private void SmoothVolume(float targetVolume, float duration)
        {
            startVolume = audioSource.volume;
            this.targetVolume = targetVolume;
            this.duration = duration;
            elapseTime = 0;
        }

        private void OnSmoothFinished()
        {
            if (state == SoundState.Stop)
            {
                audioSource.Stop();
            }
            else if (state == SoundState.Pause)
            {
                audioSource.Pause();
            }
        }

        void IUpdateable.Update(float deltaTime)
        {
            if (state == SoundState.Playing && !Loop)
            {
                if (!IsPlaying)
                {
                    state = SoundState.Stop;
                }
            }

            if (duration > 0)
            {
                elapseTime += deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapseTime / duration);
                if (elapseTime >= duration)
                {
                    duration = 0;
                    OnSmoothFinished();
                }
            }
        }

        void IClearable.Clear()
        {
            trans.position = Vector3.zero;
            audioSource.clip = null;
            volumeWhenPause = 0;
            targetVolume = 0;
            duration = 0;
            elapseTime = 0;
        }
    }
}
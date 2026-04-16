using System.Collections.Generic;
using UnityEngine;

namespace Core.Sound
{
    /// <summary>
    /// 声音组
    /// </summary>
    public class SoundGroup : IUpdateable
    {
        public string Name { get; private set; }

        /// <summary>
        /// 最大声音数量,如果为0则不限制
        /// </summary>
        public int MaxCount;

        private Transform groupRoot;

        /// <summary>
        /// 音频通道满后,是否替换相同优先级的音效
        /// </summary>
        public bool replacedBySamePriority;

        private bool mute;
        private float volume;

        private List<SoundPlayer> sounds;

        public SoundGroup(string name, int maxCount, Transform soundRoot)
        {
            Name = name;
            MaxCount = maxCount;
            GameObject go = new GameObject(name + "_" + maxCount);
            go.transform.SetParent(soundRoot);
            groupRoot = go.transform;

            replacedBySamePriority = true;
            mute = false;
            volume = 1;
            int capacity = MaxCount > 0 ? MaxCount : 0;
            sounds = new List<SoundPlayer>(capacity);
        }

        public bool Mute
        {
            get { return mute; }
            set
            {
                if (mute != value)
                {
                    mute = value;
                    foreach (var item in sounds)
                    {
                        item.RefreshMute();
                    }
                }
            }
        }

        public float Volume
        {
            get { return volume; }
            set
            {
                if (volume != value)
                {
                    volume = value;
                    foreach (var item in sounds)
                    {
                        item.RefreshVolume();
                    }
                }
            }
        }

        public void Update(float deltaTime)
        {
            foreach (var item in sounds)
            {
                ((IUpdateable)item).Update(deltaTime);
            }
        }

        public SoundPlayer PlaySound(int id, AudioClip clip, int priority, PlaySoundParams playParams)
        {
            if (null == clip)
            {
                return null;
            }

            SoundPlayer player = FindCandidatePlayer(priority);
            if (null == player)
            {
                return null;
            }

            player.Priority = priority;
            player.Time = playParams.time;
            player.Mute = playParams.mute;
            player.Loop = playParams.loop;
            player.Volume = playParams.volume;
            player.Pitch = playParams.pitch;
            player.PanStereo = playParams.panStereo;
            player.SpatialBlend = playParams.spatialBlend;
            player.ReverbZoneMix = playParams.reverbZoneMix;
            player.DopplerLevel = playParams.dopplerLevel;
            player.MaxDistance = playParams.maxDistance;
            player.MinDistance = playParams.minDistance;
            player.SetData(clip, id, playParams.finishedFadeOutTime);
            player.Play(playParams.fadeInTime);
            return player;
        }

        private SoundPlayer FindCandidatePlayer(int priority)
        {
            SoundPlayer candidate = null;
            bool isFull = MaxCount > 0 && sounds.Count >= MaxCount;
            foreach (var item in sounds)
            {
                if (!item.IsPlaying)
                {
                    candidate = item;
                    break;
                }

                if (isFull)
                {
                    if (priority < item.Priority)
                    {
                        if (null == candidate || candidate.Priority < item.Priority)
                        {
                            candidate = item;
                        }
                    }
                    else if (item.Priority == priority && replacedBySamePriority)
                    {
                        if (null == candidate || item.StartPlayTime < candidate.StartPlayTime)
                        {
                            candidate = item;
                        }
                    }
                }
            }

            if (null == candidate && !isFull)
            {
                candidate = CreateSoundPlayer();
            }

            return candidate;
        }

        private SoundPlayer CreateSoundPlayer()
        {
            GameObject go = new GameObject();
            go.transform.SetParent(groupRoot);
            SoundPlayer player = new SoundPlayer(this, go);
            sounds.Add(player);
            if (Application.isEditor)
            {
                go.name = sounds.Count.ToString();
            }

            return player;
        }

        public void AddSoundPlayer()
        {
            CreateSoundPlayer();
            int count = sounds.Count;
            if (MaxCount > 0 && MaxCount < count)
            {
                MaxCount = count;
            }
        }

        public bool StopSound(int id, float fadeOutTime)
        {
            foreach (var item in sounds)
            {
                if (item.Id == id)
                {
                    item.Stop(fadeOutTime);
                    return true;
                }
            }

            return false;
        }

        public bool PauseSound(int id, float fadeOutTime)
        {
            foreach (var item in sounds)
            {
                if (item.Id == id)
                {
                    item.Pause(fadeOutTime);
                    return true;
                }
            }

            return false;
        }

        public bool ResumeSound(int id, float fadeInTime)
        {
            foreach (var item in sounds)
            {
                if (item.Id == id)
                {
                    item.Resume(fadeInTime);
                    return true;
                }
            }

            return false;
        }

        public void PauseAllSounds()
        {
            foreach (var item in sounds)
            {
                item.Pause(0);
            }
        }

        public void ResumeAllSounds()
        {
            foreach (var item in sounds)
            {
                item.Resume(0);
            }
        }

        public void StopAllSounds()
        {
            foreach (var item in sounds)
            {
                item.Stop(0);
            }
        }
    }
}
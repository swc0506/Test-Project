using System;

namespace Core.Sound
{
    public struct PlaySoundParams : IEquatable<PlaySoundParams>
    {
        public float time;
        public bool mute;
        public bool loop;
        public float volume;
        public float pitch;
        public float panStereo;
        public float spatialBlend;
        public float reverbZoneMix;
        public float dopplerLevel;
        public float maxDistance;
        public float minDistance;
        public float fadeInTime;
        public float finishedFadeOutTime;

        public static PlaySoundParams Create()
        {
            PlaySoundParams playParams = new PlaySoundParams();
            playParams.time = 0;
            playParams.mute = false;
            playParams.loop = false;
            playParams.volume = 1;
            playParams.pitch = 1;
            playParams.panStereo = 0;
            playParams.spatialBlend = 0;
            playParams.reverbZoneMix = 0;
            playParams.dopplerLevel = 1;
            playParams.maxDistance = 100;
            playParams.minDistance = 0;
            playParams.fadeInTime = 0;
            playParams.finishedFadeOutTime = 0;
            return playParams;
        }

        public bool Equals(PlaySoundParams other)
        {
            return time.Equals(other.time) && mute == other.mute && loop == other.loop && volume.Equals(other.volume) && pitch.Equals(other.pitch) && panStereo.Equals(other.panStereo) && spatialBlend.Equals(other.spatialBlend) && reverbZoneMix.Equals(other.reverbZoneMix) && dopplerLevel.Equals(other.dopplerLevel) && maxDistance.Equals(other.maxDistance) && minDistance.Equals(other.minDistance) && fadeInTime.Equals(other.fadeInTime) && finishedFadeOutTime.Equals(other.finishedFadeOutTime);
        }

        public override bool Equals(object obj)
        {
            return obj is PlaySoundParams other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = time.GetHashCode();
                hashCode = (hashCode * 397) ^ mute.GetHashCode();
                hashCode = (hashCode * 397) ^ loop.GetHashCode();
                hashCode = (hashCode * 397) ^ volume.GetHashCode();
                hashCode = (hashCode * 397) ^ pitch.GetHashCode();
                hashCode = (hashCode * 397) ^ panStereo.GetHashCode();
                hashCode = (hashCode * 397) ^ spatialBlend.GetHashCode();
                hashCode = (hashCode * 397) ^ reverbZoneMix.GetHashCode();
                hashCode = (hashCode * 397) ^ dopplerLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ maxDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ minDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ fadeInTime.GetHashCode();
                hashCode = (hashCode * 397) ^ finishedFadeOutTime.GetHashCode();
                return hashCode;
            }
        }
    }
}
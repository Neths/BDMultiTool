using System;
using NAudio.Wave;
using NVorbis.NAudioSupport;

namespace BDMultiTool.Core.Notification
{
    public interface ISoundNotifier
    {
        void PlayNotificationSound();
    }

    public class SoundNotification : ISoundNotifier
    {
        private VorbisWaveReader _vorbisReader;
        private WaveOut _waveOut;

        public void PlayNotificationSound()
        {
            _vorbisReader = new VorbisWaveReader(BDMTConstants.WORKSPACE_PATH + BDMTConstants.NOTIFICATION_SOUND_FILE);

            _waveOut = new WaveOut();
            _waveOut.PlaybackStopped += OnPlaybackStopped;
            _waveOut.Init(_vorbisReader);
            _waveOut.Volume = 0.65f;
            _waveOut.Play();
        }

        private void OnPlaybackStopped(object sender, EventArgs e)
        {
            _waveOut.PlaybackStopped -= OnPlaybackStopped;
            _waveOut = null;

            _vorbisReader.Dispose();
            _vorbisReader = null;
        }
    }
}

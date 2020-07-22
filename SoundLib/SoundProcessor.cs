using NAudio.Wave;
using System;

namespace SoundLib
{
	public class SoundProcessor : IDisposable
	{
		private readonly FftSampleProvider _fftSampleProvider;
        private readonly IWavePlayer _playbackDevice;
        private readonly WaveStream _fileStream;

        public event Action<double[]> FftCalculated;

        public int FftSize => _fftSampleProvider.FftSize;
        public double MinAmplitudeDb => _fftSampleProvider.MinAmplitudeDb;

        public SoundProcessor(string fileName)
		{
            try
            {
                var inputStream = new AudioFileReader(fileName);
                _fileStream = inputStream;
                _fftSampleProvider = new FftSampleProvider(inputStream);
                _fftSampleProvider.FftCalculated += (result) => FftCalculated?.Invoke(result);

                _playbackDevice = new WaveOut { DesiredLatency = 200 };
                _playbackDevice.Init(_fftSampleProvider);
            }
            catch (Exception ex)
            {
                CloseFile();
                throw ex;
            }
        }

        public void Dispose()
        {
            CloseFile();
        }

        public void Play()
        {
            if (_playbackDevice != null && _fileStream != null && _playbackDevice.PlaybackState != PlaybackState.Playing)
                _playbackDevice.Play();
        }

        public void Pause()
        {
            _playbackDevice?.Pause();
        }

        public void Stop()
        {
            _playbackDevice?.Stop();
            if (_fileStream != null)
                _fileStream.Position = 0;
        }

        private void CloseFile()
        {
            Stop();
            _fileStream?.Dispose();
        }
    }
}
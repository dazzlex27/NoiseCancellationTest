using System;
using NAudio.Dsp;
using NAudio.Wave;

namespace SoundLib
{
    internal class FftSampleProvider : ISampleProvider
    {
        public event Action<double[]> FftCalculated;

        private readonly Complex[] fftBuffer;
        private readonly double[] _resultBuffer;

        private int _fftBufferPosition;
        private readonly int _mFactor;
        private readonly ISampleProvider _source;

        private readonly int _audioChannels;

        public WaveFormat WaveFormat => _source.WaveFormat;

        public int FftSize { get; }

        public double MinAmplitudeDb { get; }

        public FftSampleProvider(ISampleProvider source, int fftSize = 1024, double minAmplitudeDb = -90)
        {
            _audioChannels = source.WaveFormat.Channels;
            if (!IsPowerOfTwo(fftSize))
                throw new ArgumentException("FFT Length must be a power of two");

            FftSize = fftSize;

            _mFactor = (int)Math.Log(FftSize, 2.0);
            fftBuffer = new Complex[FftSize];
            _resultBuffer = new double[FftSize];
            _source = source;
            MinAmplitudeDb = minAmplitudeDb;
        }

        private static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }

        private void Add(float value)
        {
            if (FftCalculated == null)
                return;

            fftBuffer[_fftBufferPosition].X = (float)(value * FastFourierTransform.HammingWindow(_fftBufferPosition, FftSize));
            fftBuffer[_fftBufferPosition].Y = 0;
            _fftBufferPosition++;
            if (_fftBufferPosition >= fftBuffer.Length)
            {
                _fftBufferPosition = 0;
                // 1024 = 2^10
                FastFourierTransform.FFT(true, _mFactor, fftBuffer);
                ConvertComplexToAmplitude();
                FftCalculated(_resultBuffer);
            }
        }

        private void ConvertComplexToAmplitude()
        {
            for (var i = 0; i < FftSize; i++)
                _resultBuffer[i] = GetAmplitude(fftBuffer[i]);
        }

        public double GetAmplitude(Complex c)
        {
            const int amplitudeMultiplier = 20;
            // 20 from https://dspillustrations.com/pages/posts/misc/decibel-conversion-factor-10-or-factor-20.html
            double amplitudeDb = amplitudeMultiplier * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            if (amplitudeDb < MinAmplitudeDb)
                amplitudeDb = MinAmplitudeDb;

            return amplitudeDb;
        }

        public int Read(float[] buffer, int offset, int count)
        { 
            var samplesRead = _source.Read(buffer, offset, count);

            for (int i = 0; i < samplesRead; i += _audioChannels)
                Add(buffer[i + offset]);

            return samplesRead;
        }
    }
}
using SoundLib;
using System;
using System.Windows;
using System.Windows.Input;

namespace NoiseCancellationTestApp
{
	internal class MainWindowVm : BaseViewModel
	{
		private SoundProcessor _soundProcessor;

		private int _toneFrequency;
		private int _toneLength;
		private string _soundFilePath;
		private SpectrumVisualizerVm _spectrumVisVm;

		public ICommand PlayToneCommand { get; }
		public ICommand PlaySoundCommand { get; }
		public ICommand StopSoundCommand { get; }

		public int ToneFrequency
		{
			get => _toneFrequency;
			set => SetField(ref _toneFrequency, value, nameof(ToneFrequency));
		}

		public int ToneLength
		{
			get => _toneLength;
			set => SetField(ref _toneLength, value, nameof(ToneLength));
		}

		public string SoundFilePath
		{
			get => _soundFilePath;
			set => SetField(ref _soundFilePath, value, nameof(SoundFilePath));
		}

		public SpectrumVisualizerVm SpectrumVisVm
		{
			get => _spectrumVisVm;
			set => SetField(ref _spectrumVisVm, value, nameof(SpectrumVisVm));
		}

		public MainWindowVm()
		{
			_toneFrequency = 440;
			_toneLength = 500;
			_soundFilePath = "test.mp3";

			SpectrumVisVm = new SpectrumVisualizerVm();

			PlayToneCommand = new CommandHandler(PlayTone, true);
			PlaySoundCommand = new CommandHandler(PlaySound, true);
			StopSoundCommand = new CommandHandler(StopSound, true);
		}

		private void PlayTone()
		{
			try
			{
				Console.Beep(ToneFrequency, ToneLength);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void PlaySound()
		{
			try
			{
				_soundProcessor = new SoundProcessor(SoundFilePath);
				_soundProcessor.FftCalculated += OnFftCalculated;
				_soundProcessor.Play();
				SpectrumVisVm.UpdateParams(_soundProcessor.FftSize, _soundProcessor.MinAmplitudeDb);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void StopSound()
		{
			try
			{
				if (_soundProcessor == null)
					return;

				_soundProcessor.FftCalculated -= OnFftCalculated;
				_soundProcessor.Dispose();
				_soundProcessor = null;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void OnFftCalculated(double[] result)
		{
			SpectrumVisVm.UpdateGraph(result);
		}
	}
}

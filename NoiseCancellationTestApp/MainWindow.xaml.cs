using SoundLib;
using System;
using System.Windows;

namespace NoiseCancellationTestApp
{
	internal partial class MainWindow : Window
	{
		private SoundProcessor _soundProcessor;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void OnFftCalculated(double[] result)
		{
			SpetrumVis.UpdateGraph(result);
		}

		private void OnPlayToneClicked(object sender, RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(TbFr.Text) || string.IsNullOrEmpty(TbLg.Text))
					return;

				var frequency = int.Parse(TbFr.Text);
				var length = int.Parse(TbLg.Text);
				Console.Beep(frequency, length);
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void OnPlaySoundClicked(object sender, RoutedEventArgs e)
		{
			try
			{
				_soundProcessor = new SoundProcessor(TbAudioFile.Text);
				_soundProcessor.FftCalculated += OnFftCalculated;
				_soundProcessor.Play();
				SpetrumVis.UpdateParams(_soundProcessor.FftSize, _soundProcessor.MinAmplitudeDb);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void OnStopSoundClicked(object sender, RoutedEventArgs e)
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
	}
}
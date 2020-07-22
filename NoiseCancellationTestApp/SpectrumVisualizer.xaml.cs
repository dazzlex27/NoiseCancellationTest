using System.Windows;
using System.Windows.Controls;

namespace NoiseCancellationTestApp
{
    internal partial class SpectrumVisualizer : UserControl
    {
        private const int ThinningFactorX = 2;

        private int _binCount;
        private double _minAmplitudeDb;
        private double _scaleX;
        //private int _updateCount;

        public SpectrumVisualizer()
        {
            InitializeComponent();
            CalculateXScale();
            SizeChanged += OnControlSizeChanged;
        }

        public void UpdateParams(int fftSize, double minAmplitudeDb)
        {
            _binCount = fftSize / 2;
            _minAmplitudeDb = minAmplitudeDb;
            CalculateXScale();
        }

        public void UpdateGraph(double[] fftResults)
        {
            // no need to repaint too many frames per second
            //if (_updateCount++ % 2 == 0)
           //     return;

            for (int n = 0; n < fftResults.Length / 2; n += ThinningFactorX)
            {
                double yPos = 0;
                for (int b = 0; b < ThinningFactorX; b++)
                {
                    var amplitudeDb = fftResults[n + b];
                    yPos += amplitudeDb / _minAmplitudeDb * ActualHeight;
                }

                AddResult(n / ThinningFactorX, yPos / ThinningFactorX);
            }
        }

        private void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateXScale();
        }

        private void CalculateXScale()
        {
            if (_binCount == 0)
            {
                _scaleX = 0;
                return;
            }

            _scaleX = ActualWidth / (_binCount / ThinningFactorX);
        }

        private void AddResult(int index, double amplitudeDb)
        {
            var x = index * _scaleX; // Math.Log10(index) * _scaleX;
            var linePoint = new Point(x, amplitudeDb);
            if (index >= polyline1.Points.Count)
                polyline1.Points.Add(linePoint);
            else
                polyline1.Points[index] = linePoint;
        }
    }
}
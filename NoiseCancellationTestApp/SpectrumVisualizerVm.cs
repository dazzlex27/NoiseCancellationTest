using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace NoiseCancellationTestApp
{
	internal class SpectrumVisualizerVm : BaseViewModel
	{
		private const int ThinningFactorX = 2;

        private double _actualWidth;
        private double _actualHeight;

		private int _binCount;
		private double _minAmplitudeDb;
		private double _scaleX;

        private PointCollection _polylinePoints;

        public PointCollection PolylinePoints
        {
            get => _polylinePoints;
            set => SetField(ref _polylinePoints, value, nameof(PolylinePoints));
        }

        public SpectrumVisualizerVm()
        {
            PolylinePoints = new PointCollection();
        }

        public void UpdateActualSize(double actualWidth, double actualHeight)
        {
            _actualWidth = actualWidth;
            _actualHeight = actualHeight;
            CalculateScaleX();
        }

        public void UpdateParams(int fftSize, double minAmplitudeDb)
        {
            _binCount = fftSize / 2;
            _minAmplitudeDb = minAmplitudeDb;
            CalculateScaleX();
        }

        public void UpdateGraph(double[] fftResults)
        {
            var sizeToReserve = fftResults.Length / (2 * ThinningFactorX);
            var newGraphPoints = new List<Point>(sizeToReserve);

            for (int i = 0; i < fftResults.Length / 2; i += ThinningFactorX)
            {
                double y = 0;
                for (int j = 0; j < ThinningFactorX; j++)
                {
                    var amplitudeDb = fftResults[i + j];
                    y += amplitudeDb / _minAmplitudeDb * _actualHeight;
                }

                var index = i / ThinningFactorX;
                var amplitudeToDisplay = y / ThinningFactorX;

                var x = index * _scaleX; // Math.Log10(index) * _scaleX;
                var linePoint = new Point(x, amplitudeToDisplay);
                if (index >= newGraphPoints.Count)
                    newGraphPoints.Add(linePoint);
                else
                    newGraphPoints[index] = linePoint;
            }

            PolylinePoints = new PointCollection(newGraphPoints);
        }

        private void CalculateScaleX()
        {
            if (_binCount == 0)
            {
                _scaleX = 0;
                return;
            }

            _scaleX = _actualWidth / (_binCount / ThinningFactorX);
        }
    }
}
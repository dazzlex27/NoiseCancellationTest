using System.Windows;
using System.Windows.Controls;

namespace NoiseCancellationTestApp
{
    internal partial class SpectrumVisualizer : UserControl
    {
        public SpectrumVisualizer()
        {
            InitializeComponent();
            SizeChanged += OnControlSizeChanged;
        }

        private void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var vm = DataContext as SpectrumVisualizerVm;
            if (vm == null)
                return;

            vm.UpdateActualSize(ActualWidth, ActualHeight);
        }
    }
}
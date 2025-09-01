using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace hourglass
{
    public partial class MainWindow : Window
    {
        private double currentPercent = 0; // start at 50%

        int state = 0;
        public MainWindow()
        {
            InitializeComponent();
            SetProgress(currentPercent);
        }

        private void ChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Background == Brushes.White)
                MainGrid.Background = Brushes.LightBlue;
            else
                MainGrid.Background = Brushes.White;
        }

        private void Increase_Click(object sender, RoutedEventArgs e)
        {
            AnimateProgress(currentPercent, Math.Min(100, currentPercent += 20));
        }

        private void Decrease_Click(object sender, RoutedEventArgs e)
        {
            AnimateProgress(currentPercent, Math.Max(0, currentPercent -= 20));
        }

        private void AnimateProgress(double from, double to)
        {
            DoubleAnimation animation = new DoubleAnimation(from, to, TimeSpan.FromSeconds(1));
            animation.EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut };
            animation.FillBehavior = FillBehavior.Stop;
            animation.Completed += (s, e) => { currentPercent = to; SetProgress(currentPercent); };

            // Apply animation using a dummy dependency property
            this.BeginAnimation(ProgressValueProperty, animation);
        }

        // DependencyProperty to drive animation
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(MainWindow),
                new PropertyMetadata(0.0, OnProgressValueChanged));

        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }

        private static void OnProgressValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow window)
            {
                double percent = (double)e.NewValue;
                window.SetProgress(percent);
            }
        }

        private void SetProgress(double percent)
        {
            percent = Math.Max(0, Math.Min(100, percent));
            double angle = percent / 100 * 360;

            double radius = 80;
            double centerX = 100;
            double centerY = 100;

            double radians = (Math.PI / 180) * (angle - 90);
            double x = centerX + radius * Math.Cos(radians);
            double y = centerY + radius * Math.Sin(radians);

            bool largeArc = angle > 180;

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(centerX, centerY - radius);

            ArcSegment arc = new ArcSegment
            {
                Point = new Point(x, y),
                Size = new Size(radius, radius),
                IsLargeArc = largeArc,
                SweepDirection = SweepDirection.Clockwise
            };

            figure.Segments.Add(arc);

            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            ProgressArc.Data = geometry;
            PercentageText.Text = $"{percent:0}%";
        }
    }
}

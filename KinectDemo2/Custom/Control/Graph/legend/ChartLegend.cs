using Windows.UI.Popups;

namespace KinectDemo2.Custom.Control.Graph.legend
{
    /// <summary>
    /// The items in the legend contain the key information about the ChartSeries. The legend has all abilities such as docking, enabling, or disabling the desired series.
    /// </summary>
    public class ChartLegend : BindableObject
    {
        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(ChartLegend), true);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ChartLegend));

        /// <summary>
        /// Legends can be placed left, right, top, or bottom around the chart area.
        /// </summary>
        public static readonly BindableProperty PlacementProperty =
            BindableProperty.Create(nameof(Placement), typeof(LegendPlacement), typeof(ChartLegend), LegendPlacement.Top);

        public static readonly BindableProperty ToggleSeriesVisibilityProperty =
            BindableProperty.Create(nameof(ToggleSeriesVisibility), typeof(bool), typeof(ChartLegend), false);

        public bool IsVisible { get; set; }
        public DataTemplate ItemTemplate { get; set; }
        public LegendPlacement Placement { get; set; }
        public bool ToggleSeriesVisibility { get; set; }

    }
}

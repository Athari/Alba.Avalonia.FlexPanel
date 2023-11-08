using CommunityToolkit.Mvvm.ComponentModel;

namespace Alba.Avalonia.FlexPanel.Demo.ViewModels;

public sealed partial class ItemModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private FlexItemAlignment _alignSelf = FlexItemAlignment.Auto;

    [ObservableProperty]
    private int _order;

    [ObservableProperty]
    private double _flexGrow;

    [ObservableProperty]
    private double _flexShink;

    public int Value { get; }

    public ItemModel(int value) =>
        Value = value;
}
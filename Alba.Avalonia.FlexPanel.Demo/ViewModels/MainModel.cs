using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Alba.Avalonia.FlexPanel.Demo.ViewModels;

public sealed partial class MainModel : ObservableObject
{
    private readonly ObservableCollection<ItemModel> _numbers;

    [ObservableProperty]
    private bool _isItemsControl = true;

    [ObservableProperty]
    private bool _isItemsRepeater;

    [ObservableProperty]
    private FlexDirection _direction = FlexDirection.Row;

    [ObservableProperty]
    private FlexContentJustify _justifyContent = FlexContentJustify.FlexStart;

    [ObservableProperty]
    private FlexItemsAlignment _alignItems = FlexItemsAlignment.FlexStart;

    [ObservableProperty]
    private FlexContentAlignment _alignContent = FlexContentAlignment.FlexStart;

    [ObservableProperty]
    private FlexWrap _wrap = FlexWrap.Wrap;

    [ObservableProperty]
    private int _columnGap = 80;

    [ObservableProperty]
    private int _rowGap = 32;

    private int _currentNumber = 41;

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RemoveItemCommand))]
    private ItemModel? _selectedItem;

    public IEnumerable DirectionValues { get; } = Enum.GetValues(typeof(FlexDirection));

    public IEnumerable JustifyContentValues { get; } = Enum.GetValues(typeof(FlexContentJustify));

    public IEnumerable AlignItemsValues { get; } = Enum.GetValues(typeof(FlexItemsAlignment));

    public IEnumerable AlignContentValues { get; } = Enum.GetValues(typeof(FlexContentAlignment));

    public IEnumerable WrapValues { get; } = Enum.GetValues(typeof(FlexWrap));

    public IEnumerable AlignSelfValues { get; } = Enum.GetValues(typeof(FlexItemAlignment));

    public ReadOnlyObservableCollection<ItemModel> Numbers { get; }

    public IRelayCommand AddItemCommand { get; }

    public IRelayCommand RemoveItemCommand { get; }

    public MainModel()
    {
        _numbers = new(Enumerable.Range(1, 40).Select(x => new ItemModel(x)));
        Numbers = new(_numbers);
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand(RemoveItem, () => SelectedItem != null);
    }

    private void AddItem()
    {
        _numbers.Add(new(_currentNumber++));
    }

    private void RemoveItem()
    {
        if (SelectedItem is null)
            return;
        _numbers.Remove(SelectedItem);
        SelectedItem.IsSelected = false;
        if (Numbers.Any()) {
            SelectedItem = Numbers.Last();
            SelectedItem.IsSelected = true;
        }
    }
}
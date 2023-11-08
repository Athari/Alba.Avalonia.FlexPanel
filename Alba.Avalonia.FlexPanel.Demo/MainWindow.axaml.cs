using System;
using Alba.Avalonia.FlexPanel.Demo.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Alba.Avalonia.FlexPanel.Demo;

public sealed partial class MainWindow : Window
{
    public static readonly StyledProperty<MainModel> ModelProperty =
        AvaloniaProperty.Register<MainWindow, MainModel>(nameof(Model));

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Model = (MainModel)DataContext!;
    }

    private void OnItemTapped(object? sender, RoutedEventArgs e)
    {
        if (sender is not ListBoxItem { DataContext: ItemModel item })
            return;
        if (Model.SelectedItem != null)
            Model.SelectedItem.IsSelected = false;
        if (Model.SelectedItem == item)
            Model.SelectedItem = null;
        else {
            Model.SelectedItem = item;
            Model.SelectedItem.IsSelected = true;
        }
    }
}
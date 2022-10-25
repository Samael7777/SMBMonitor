using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace SMBMonitor.View;

public class ListBoxSelectedItemBehaviour : Behavior<ListBox>
{
    //dependency property
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register("SelectedItems", typeof(IList),
            typeof(ListBoxSelectedItemBehaviour),
            new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });
  
    //property wrapper
    public IList SelectedItems
    {
        get => (IList)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }
  
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnListBoxSelectedItemChanged;
    }

    private void OnListBoxSelectedItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
            SelectedItems = listBox.SelectedItems;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
            AssociatedObject.SelectionChanged -= OnListBoxSelectedItemChanged;
    }
}

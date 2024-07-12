using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Notes.Selectors;

public class TemplateSelector : DataTemplateSelector
{
    public DataTemplate ItemTemplate { get; set; }
    public DataTemplate NewItemTemplate { get; set; }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item == CollectionView.NewItemPlaceholder ? NewItemTemplate : ItemTemplate;
    }
}
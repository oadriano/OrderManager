using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;


namespace OrderManager.ViewModel
{
    public class DeselectOnEmptyAreaBehavior : Behavior<DataGrid>
    {
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataGrid = sender as DataGrid;

            if (dataGrid != null && dataGrid.SelectedItem != null)
            {
                if (e.OriginalSource is ScrollViewer)
                {
                    ((DataGrid)sender).UnselectAll();
                }
            }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
        }
    }
}

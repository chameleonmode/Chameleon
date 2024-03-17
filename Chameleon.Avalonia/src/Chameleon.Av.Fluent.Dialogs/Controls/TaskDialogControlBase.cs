using Avalonia;
using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Dialogs.Controls;

//public class TaskDialogControlBase : UserControl, 
//    ITaskDialogView
//{
//    public T? FindTControl<T>(string name) where T : class
//    {
//        throw new NotImplementedException();
//    }

//    public Task<object?> ShowTDialogAsync(string name)
//    {
//        Visual? root = (ApplicationHelper.GetMainWindow()?.Content as Visual) ?? ApplicationHelper.GetToplevetVisual();
//        var td = new TaskDialog
//        {
//            Title = _apiInActionTD.Title,
//            Header = _apiInActionTD.Header,
//            SubHeader = _apiInActionTD.SubHeader,
//            Content = _content, //_apiInActionTD.Content,
//            IconSource = _apiInActionTD.IconSource,
//            ShowProgressBar = _apiInActionTD.ShowProgressBar,
//            FooterVisibility = _apiInActionTD.FooterVisibility,
//            IsFooterExpanded = _apiInActionTD.IsFooterExpanded,
//            Footer = _apiInActionTD.Content,
//            DataContext = _apiInActionTD.DataContext,
//        };
//    }
//}

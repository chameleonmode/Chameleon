
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.Services;

namespace Chameleon.Av.Fluent.Dialogs.Base;
public class ContentDialogViewModelBase<T> : ContentDialogAwareBase, IViewAware
{
    public override async Task<IContentDialogResult> ShowAsync()
    {
        return await ContentDialogService.ShowContentDialogAsync(typeof(T));
    }

    public T1 GetView<T1>()
    {
       throw new NotImplementedException();
    }
}


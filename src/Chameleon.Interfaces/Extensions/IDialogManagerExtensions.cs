using System;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.Extensions
{
    public static class IDialogManagerExtensions
    {
        //public static IDialog CreateDialog<TDialog, TViewModel>(
        //    this IDialogManager self,
        //    Action<TViewModel> initialize
        //    )
        //{
        //    var dialog = self.Create(typeof(TDialog));
        //    var dialogAware = dialog.GetDialogViewModel();
        //    if (dialogAware is TViewModel viewModel)
        //    {
        //        initialize(viewModel);
        //    }
        //    else
        //    {
        //        throw new InvalidCastException(
        //            $"{dialogAware.GetType().Name} do not implement {typeof(TViewModel).Name}");
        //    }
        //    return dialog;
        //}

        //public static IDialog CreateDialog<T>(this IDialogManager self, IUserProfile userProfile)
        //{
        //    return self.CreateDialog<T, IUserProfileSetter>(viewModel
        //       => viewModel.UserProfile = userProfile
        //    );
        //}

        //public static IDialog CreateDialog<TDialog, TViewModel, TProfile>(
        //    this IDialogManager self,
        //    Action<TViewModel> initialize
        //    , TProfile profile
        //    )
        //{
        //    var dialog = self.Create(typeof(TDialog));
        //    var dialogAware = dialog.GetDialogViewModel();
        //    if (dialogAware is TViewModel viewModel)
        //    {
        //        initialize(viewModel);
        //    }
        //    else
        //    {
        //        throw new InvalidCastException(
        //            $"{dialogAware.GetType().Name} do not implement {typeof(TViewModel).Name}");
        //    }
        //    return dialog;
        //}
    }
}

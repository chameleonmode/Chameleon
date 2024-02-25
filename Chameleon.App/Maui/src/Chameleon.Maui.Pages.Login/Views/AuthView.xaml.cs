using Chameleon.Interfaces.Auth;
using Chameleon.Maui.Pages.Login.ViewModels;
using Chameleon.Maui.Toolkit.Models;
using CommunityToolkit.Maui.Views;
using Chameleon.Prism.Events;

namespace Chameleon.Maui.Pages.Login.Views;

public partial class AuthView : Popup
{
    private readonly IEventAggregator _eventAggregator;
    public AuthView(PopupSizeConstants popupSizeConstants, AuthViewModel vm, IEventAggregator eventAggregator)
    {
        InitializeComponent();

        BindingContext = vm;

        Size = popupSizeConstants.Large;

        _eventAggregator = eventAggregator;
        _eventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(Close, ThreadOption.UIThread);

        _eventAggregator
            .GetEvent<LoginCancelEvent>()
            .SubscribeOnce(Close, ThreadOption.UIThread);
    }
}
using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Avalonia.Controls.Settings.Functional.ViewModels;
using FluentAvalonia.UI.Media.Animation;
using System;

namespace Chameleon.Avalonia.Controls.Settings.Functional.Views
{
    public partial class FunctionalSettingsView : ChameleonNavigationPage,
        IFunctionalSettingsView
    {
        public FunctionalSettingsView()
        {
            InitializeComponent();

            TabStrip1.SelectionChanged += TabStrip1SelectionChanged;
        }

        public override void OnAfterNavigatedToViewModel(object param)
        {
            base.OnAfterNavigatedToViewModel(param);
            if (DataContext is not FunctionalSettingsViewModel vm)
                return;

            if (param is IUserProfileFolder)
                InnerNavFrame.Navigate(typeof(UserProxySettingsView), param, GetTransitionInfo(vm.LastSelectedIndex, 0));
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            TabStrip1SelectionChanged(null, null);
        }

        private void TabStrip1SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is not FunctionalSettingsViewModel vm)
                return;

            var idx = TabStrip1.SelectedIndex;

            InnerNavFrame.Navigate(idx switch
            {
                0 => typeof(UserProxySettingsView),
                1 => typeof(UserDefaultSettingsView),
                2 => typeof(PhoneVerificationView),
                3 => typeof(ProxyCreditView),
                4 => typeof(AssistantUsersView),
                _ => throw new Exception()
            }, null, GetTransitionInfo(vm.LastSelectedIndex, idx));

            vm.LastSelectedIndex = TabStrip1.SelectedIndex;
        }

        private NavigationTransitionInfo GetTransitionInfo(int oldIndex, int newIndex)
        {
            SlideNavigationTransitionEffect GetEffect(int oldIndex, int index)
            {
                if (oldIndex < 0)
                    return SlideNavigationTransitionEffect.FromBottom;

                if (oldIndex > index)
                    return SlideNavigationTransitionEffect.FromRight;
                else
                    return SlideNavigationTransitionEffect.FromLeft;
            }

            if (oldIndex == -1)
            {
                return new SuppressNavigationTransitionInfo();
            }
            else
            {
                return new SlideNavigationTransitionInfo
                {
                    Effect = GetEffect(oldIndex, newIndex),
                    FromHorizontalOffset = 70
                };
            }
        }
    }
}

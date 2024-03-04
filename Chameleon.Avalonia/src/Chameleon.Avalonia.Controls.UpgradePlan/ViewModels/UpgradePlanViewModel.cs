using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Util;
using Chameleon.Interfaces.DialogWindows;
using Chameleon.Interfaces.Environments;
using Chameleon.Interfaces.UpgradePlan;
using Chameleon.Prism.Events;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace Chameleon.Avalonia.Controls.UpgradePlan.ViewModels
{
    public class UpgradePlanViewModel
        : ViewModelBase,
        IUpgradePlanViewModel
    {
        private readonly IUrlConfiguration _configuration;
        private readonly IEventAggregator _eventAggregator;
        public UpgradePlanViewModel(
            IUrlConfiguration configuration,
            IEventAggregator eventAggregator)
        {
            _configuration = configuration;
            _eventAggregator = eventAggregator;

            UpgradeCommand = new DelegateCommand(Upgrade);
        }


        public DelegateCommand UpgradeCommand { get; }
        private void Upgrade()
        {
            ProcessesUtil.GoToUrlDefault(_configuration.PricingUrl);

            _eventAggregator
                .GetEvent<CloseDialogWindowEvent>()
                .Publish((int)ButtonResult.OK);
        }


        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _limitExceededText;
        public string LimitExceededText
        {
            get => _limitExceededText;
            set => SetProperty(ref _limitExceededText, value);
        }
    }
}

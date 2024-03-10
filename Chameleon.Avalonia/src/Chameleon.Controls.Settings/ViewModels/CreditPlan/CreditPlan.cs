using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.CreditPlan;

public class CreditPlan
    : ViewModelBase
{
    private readonly IEventAggregator _eventAggregator;
    public CreditPlan(IEventAggregator eventAggregator, decimal amount, string size)
    {
        _eventAggregator = eventAggregator;
        Amount = amount;
        Size = size;
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set => SetProperty(ref _amount, value);
    }

    private string _size;
    public string Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value) && value)
            {
                _eventAggregator
                    .GetEvent<SelectedCreditPlanEvent>()
                    .Publish(new SelectedCreditPlanEventArgs(IsChecked));
            }
        }
    }
}

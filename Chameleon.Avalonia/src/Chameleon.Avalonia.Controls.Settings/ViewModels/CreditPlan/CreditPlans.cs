using Chameleon.Prism.Events;
using System.Collections.ObjectModel;

namespace Chameleon.Avalonia.Controls.Settings.ViewModels.CreditPlan;

public class CreditPlans
    : ObservableCollection<CreditPlan>
{
    private readonly IEventAggregator _eventAggregator;
    public CreditPlans(IEventAggregator eventAggregator)
    {
        _eventAggregator = eventAggregator;

        Add(new CreditPlan(_eventAggregator, 19, "5GB"));
        Add(new CreditPlan(_eventAggregator, 29, "10GB"));
        Add(new CreditPlan(_eventAggregator, 49, "20GB"));
    }
}

using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Interfaces.App.OutReach;
using Chameleon.Interfaces.OutReach;
using Chameleon.Prism.Events;
using Prism.Commands;

namespace Chameleon.Avalonia.Controls.OutReachTemplate.ViewModels;

public class OutReachTemplateItemViewModel
       : ViewModelBase
       , IOutReachTemplateItemViewModel
{
    private readonly IOutReachTemplateService _outReachTemplateService;
    private readonly IEventAggregator _eventAggregator;

    public OutReachTemplateItemViewModel(
        IOutReachTemplateService outReachTemplateService
        , IEventAggregator eventAggregator
        , IOutReachTemplate template)
    {
        _outReachTemplateService = outReachTemplateService;
        _eventAggregator = eventAggregator;
        OutReachTemplate = template;

        UseTemplateCommand = new DelegateCommand<string>(OnUseTemplate);
        DeleteTemplateCommand = new DelegateCommand(OnDeleteTemplate);
        SelectCommand = new DelegateCommand(OnSelect);
        EditCommand = new DelegateCommand(OnEdit);
        SaveCommand = new DelegateCommand(OnSave);

        InitilizePropertties();
    }

    private void InitilizePropertties()
    {
        ItemName = OutReachTemplate.Name;
        ItemContent = OutReachTemplate.Content;
    }

    public DelegateCommand<string> UseTemplateCommand { get; }
    private void OnUseTemplate(string content)
    {
        _eventAggregator
            .GetEvent<UpdateOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));
    }

    private void OnDeleteTemplate()
    {
        IsSelected = false;
        _eventAggregator
            .GetEvent<DeleteOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));

        _outReachTemplateService.Delete(OutReachTemplate);
    }

    public DelegateCommand DeleteTemplateCommand { get; }

    private IOutReachTemplate _outReachTemplate;
    public IOutReachTemplate OutReachTemplate
    {
        get => _outReachTemplate;
        set => SetProperty(ref _outReachTemplate, value);
    }

    private string _itemName;
    public string ItemName
    {
        get => _itemName;
        set
        {
            if (SetProperty(ref _itemName, value))
            {
                OutReachTemplate.Name = _itemName;
            }
        }
    }

    private string _itemContent;
    public string ItemContent
    {
        get => _itemContent;
        set => SetProperty(ref _itemContent, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public DelegateCommand SelectCommand { get; }
    public void OnSelect()
    {
        _eventAggregator
            .GetEvent<UnselectAllTemplateEvent>()
            .Publish(new UnselectAllTemplateEventArgs(OutReachTemplate.Id));

        IsSelected = true;
        OnUseTemplate(OutReachTemplate.Content);
    }

    private bool _isEdit;
    public bool IsEdit
    {
        get => _isEdit;
        set => SetProperty(ref _isEdit, value);
    }

    public DelegateCommand EditCommand { get; }
    public void OnEdit()
    {
        IsEdit = true;
    }

    public DelegateCommand SaveCommand { get; }
    private void OnSave()
    {
        OnUseTemplate(OutReachTemplate.Content);
        _eventAggregator
            .GetEvent<SaveOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));
    }
}

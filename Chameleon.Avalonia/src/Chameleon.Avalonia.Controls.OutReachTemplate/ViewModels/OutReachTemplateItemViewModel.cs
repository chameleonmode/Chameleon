using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.OutReach;
using Chameleon.Interfaces.OutReach;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;

namespace Chameleon.Avalonia.Controls.OutReachTemplate.ViewModels;

public partial class OutReachTemplateItemViewModel
       : SubPageViewModelBase
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


        InitilizePropertties();
    }

    private void InitilizePropertties()
    {
        ItemName = OutReachTemplate.Name;
        ItemContent = OutReachTemplate.Content;
    }

    [RelayCommand]
    private void OnUseTemplate(string content)
    {
        _eventAggregator
            .GetEvent<UpdateOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));
    }

    [RelayCommand]
    private void OnDeleteTemplate()
    {
        IsSelected = false;
        _eventAggregator
            .GetEvent<DeleteOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));

        _outReachTemplateService.Delete(OutReachTemplate);
    }


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


    [RelayCommand]
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

    [RelayCommand]
    public void OnEdit()
    {
        IsEdit = true;
    }

    [RelayCommand]
    private void OnSave()
    {
        OnUseTemplate(OutReachTemplate.Content);
        _eventAggregator
            .GetEvent<SaveOutReachTemplateEvent>()
            .Publish(new OutReachTemplateEventArgs(OutReachTemplate));
    }
}

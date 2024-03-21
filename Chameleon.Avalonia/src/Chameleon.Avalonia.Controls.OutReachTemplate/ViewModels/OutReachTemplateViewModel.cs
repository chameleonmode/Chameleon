using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.CT.Common.Base;
using Chameleon.Interfaces.App.OutReach.Views;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Chameleon.Avalonia.Controls.OutReachTemplate.ViewModels;

public partial class OutReachTemplateViewModel
       : SubPageViewModelBase
       , IOutReachTemplateViewModel
{
    private const string NewTemplateName = "New Template";

    private readonly IOutReachTemplateService _outReachTemplateService;
    private readonly IEventAggregator _eventAggregator;

    private ObservableCollection<IOutReachTemplate, OutReachTemplateItemViewModel> _mapping;

    public OutReachTemplateViewModel(
        IOutReachTemplateService outReachTemplateService
        , IEventAggregator eventAggregator
        , IOutReachTemplate template = null)
    {
        _eventAggregator = eventAggregator;
        _outReachTemplateService = outReachTemplateService;

        //_eventAggregator
        //    .GetEvent<LoginSuccessEvent>()
        //    .SubscribeOnce(OnInitializeViewModelsAsync());

        _eventAggregator
            .GetEvent<UpdateOutReachTemplateEvent>()
            .Subscribe(args => UpdateOutReachTemplate(args.OutReachTemplate));

        _eventAggregator
            .GetEvent<DeleteOutReachTemplateEvent>()
            .Subscribe(args => OnDeleteTemplate(args.OutReachTemplate));

        _eventAggregator
            .GetEvent<UnselectAllTemplateEvent>()
            .Subscribe(args => OnUnselectAll(args.OutReachTemplateId));

        _eventAggregator
            .GetEvent<SaveOutReachTemplateEvent>()
            .Subscribe(args => OnSave());

        InitItemModel(template);
    }
    public override async Task InitAsync()
    {
        await base.InitAsync();

        await OnInitializeViewModelsAsync();
    }

    private void OnSave()
    {
        OnSaveOutReachTemplate();
    }

    [RelayCommand]
    private void OnOpenOutReachView()
    {
        foreach (var item in ViewModels)
        {
            item.IsSelected = false;
            item.IsEdit = false;
            item.ItemName = item.OutReachTemplate.Name;
            item.ItemContent = item.OutReachTemplate.Content;
        }

        Subject = null;
        Content = null;

        OnPropertyChanged(nameof(Subject));
        OnPropertyChanged(nameof(Content));
        OnPropertyChanged(nameof(HasSelectedEmailTemplate));

        _eventAggregator
            .GetEvent<OpenUserOutReachEvent>()
            .Publish(new UserProfileEventArgs(UserProfile));
    }

    private void OnUnselectAll(int outReachTemplateId)
    {
        foreach (var item in ViewModels)
        {
            if (item.OutReachTemplate.Id == outReachTemplateId)
            {
                continue;
            }

            item.IsSelected = false;
            item.IsEdit = false;
            item.ItemName = item.OutReachTemplate.Name;
        }
        OnPropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    public bool HasSelectedEmailTemplate
        => ViewModels?.Any(a => a.IsSelected) ?? false;

    private void UpdateOutReachTemplate(IOutReachTemplate template)
    {
        Content = template.Content;
        Name = template.Name;
        Id = template.Id;
        Subject = template.Subject;

        OnPropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    private void InitItemModel(IOutReachTemplate template)
    {
        if (template == null)
        {
            return;
        }

        Content = template.Content;
        Name = template.Name;
        Subject = template.Subject;
    }

    private bool CanSaveTemplate()
    {
        return !string.IsNullOrEmpty(Name?.Trim());
    }

    [RelayCommand]
    private void OnCreateOutReachTemplate()
    {
        var outReachTemplate = new Domain.Entities.OutReachTemplate
        {
            Content = Content,
            Name = GetName(),
            Id = Id,
            Subject = Subject
        };

        var template = _outReachTemplateService.Create(outReachTemplate);
        var model = ViewModels.FirstOrDefault(a => a.OutReachTemplate.Id == template.Id);
        model.SelectCommand.Execute(null);
    }

    private string GetName()
    {
        for (var i = 0; true; ++i)
        {
            var prefix = i > 0 ? i.ToString() : string.Empty;
            var name = $"{NewTemplateName} {prefix}";

            if (!ViewModels.Any(a => a.OutReachTemplate.Name == name))
            {
                return name;
            }
        }
    }

    [RelayCommand]
    private void OnSendOutReachTemplate()
    {
        // TODO: move to separate service
        var mailto = $"mailto:{ContactEmail}?Subject={Subject}&Body={Content}";
        mailto = Uri.EscapeUriString(mailto);
        Process.Start(new ProcessStartInfo(mailto) { UseShellExecute = true });
    }

    private void OnDeleteTemplate(IOutReachTemplate template)
    {
        if (Id == template.Id)
        {
            Id = 0;
        }

        OnPropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    [RelayCommand]  //TODO CanSaveTemplate
    private async Task OnSaveOutReachTemplate()
    {
        var outReachTemplate = new Domain.Entities.OutReachTemplate
        {
            Content = Content,
            Name = Name,
            Id = Id,
            Subject = Subject
        };

        if (Id > 0)
        {
            _outReachTemplateService.Save(outReachTemplate);
        }
        else
        {
            _outReachTemplateService.Create(outReachTemplate);
            Id = outReachTemplate.Id;
        }

        await RefreshViewModels();

        var model = ViewModels.FirstOrDefault(a => a.OutReachTemplate.Id == Id);
        model.SelectCommand.Execute(null);

        OnPropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    private Task RefreshViewModels()
    {
        _viewModels?.Clear();
        _viewModels = null;

        return OnInitializeViewModelsAsync();
    }

    private ObservableCollectionView<OutReachTemplateItemViewModel> _viewModels;
    public ObservableCollectionView<OutReachTemplateItemViewModel> ViewModels
    {
        get
        {
            if (_viewModels == null && _mapping != null)
            {
                _viewModels = new ObservableCollectionView<OutReachTemplateItemViewModel>(_mapping)
                {
                    TrackItemChanges = true
                };
            }
            return _viewModels;
        }
    }

    private async Task OnInitializeViewModelsAsync()
    {
        var outReachTemplates = await Task.Run(()=>_outReachTemplateService.GetAll());

        _mapping = new ObservableCollection<IOutReachTemplate, OutReachTemplateItemViewModel>(
            outReachTemplates, template => new OutReachTemplateItemViewModel(_outReachTemplateService, _eventAggregator, template)
            );

        OnPropertyChanged(nameof(ViewModels));
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _subject;
    public string Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    public string ContactEmail => OutReachTemplate?.ContactEmail;

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (SetProperty(ref _name, value))
            {
                //SaveOutReachTemplateCommand.RaiseCanExecuteChanged();
                //CreateOutReachTemplateCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private string _content;
    public string Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    private IOutReachTemplate _outReachTemplate;
    public IOutReachTemplate OutReachTemplate
    {
        get => _outReachTemplate;
        set
        {
            if (SetProperty(ref _outReachTemplate, value))
            {
                OnPropertyChanged(nameof(ContactName));
                OnPropertyChanged(nameof(ContactEmail));

                Name = value.Name;
                Subject = value.Subject;
                Content = value.Content;
            }
        }
    }
    public string ContactName => OutReachTemplate?.ContactName;

    private string _userProfileTitle;
    public string UserProfileTitle
    {
        get => _userProfileTitle;
        set => SetProperty(ref _userProfileTitle, value);
    }

    private IUserProfile _userProfile;
    public IUserProfile UserProfile
    {
        get => _userProfile;
        set => SetProperty(ref _userProfile, value);
    }
}
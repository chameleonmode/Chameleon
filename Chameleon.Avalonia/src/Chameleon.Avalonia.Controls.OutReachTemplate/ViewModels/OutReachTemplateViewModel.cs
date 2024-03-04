using Chameleon.Avalonia.Prism.Module.Base;
using Chameleon.Core.Collections;
using Chameleon.Core.Collections.Views;
using Chameleon.Interfaces.App.OutReach.Views;
using Chameleon.Interfaces.Auth;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Prism.Events;
using Prism.Commands;
using System.Diagnostics;

namespace Chameleon.Avalonia.Controls.OutReachTemplate.ViewModels;

public class OutReachTemplateViewModel
       : ViewModelBase
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

        SendOutReachTemplateCommand = new DelegateCommand(OnSendOutReachTemplate);
        SaveOutReachTemplateCommand = new DelegateCommand(OnSaveOutReachTemplate, CanSaveTemplate);
        SaveNewOutReachTemplateCommand = new DelegateCommand(OnCreateOutReachTemplate/*, CanSaveTemplate*/);
        OpenOutReachViewCommand = new DelegateCommand(OnOpenOutReachView);

        _eventAggregator
            .GetEvent<LoginSuccessEvent>()
            .SubscribeOnce(OnInitializeViewModels);

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

    private void OnSave()
    {
        OnSaveOutReachTemplate();
    }

    public DelegateCommand OpenOutReachViewCommand { get; }
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

        RaisePropertyChanged(nameof(Subject));
        RaisePropertyChanged(nameof(Content));
        RaisePropertyChanged(nameof(HasSelectedEmailTemplate));

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
        RaisePropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    public bool HasSelectedEmailTemplate
        => ViewModels?.Any(a => a.IsSelected) ?? false;

    private void UpdateOutReachTemplate(IOutReachTemplate template)
    {
        Content = template.Content;
        Name = template.Name;
        Id = template.Id;
        Subject = template.Subject;

        RaisePropertyChanged(nameof(HasSelectedEmailTemplate));
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

    public DelegateCommand SaveNewOutReachTemplateCommand { get; }
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
        model.SelectCommand.Execute();
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

    public DelegateCommand SendOutReachTemplateCommand { get; }
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

        RaisePropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    public DelegateCommand SaveOutReachTemplateCommand { get; }
    private void OnSaveOutReachTemplate()
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

        RefreshViewModels();

        var model = ViewModels.FirstOrDefault(a => a.OutReachTemplate.Id == Id);
        model.SelectCommand.Execute();

        RaisePropertyChanged(nameof(HasSelectedEmailTemplate));
    }

    private void RefreshViewModels()
    {
        _viewModels?.Clear();
        _viewModels = null;

        OnInitializeViewModels();
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

    private void OnInitializeViewModels()
    {
        var outReachTemplates = _outReachTemplateService.GetAll();

        _mapping = new ObservableCollection<IOutReachTemplate, OutReachTemplateItemViewModel>(
            outReachTemplates, template => new OutReachTemplateItemViewModel(_outReachTemplateService, _eventAggregator, template)
            );

        RaisePropertyChanged(nameof(ViewModels));
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
                SaveOutReachTemplateCommand.RaiseCanExecuteChanged();
                SaveNewOutReachTemplateCommand.RaiseCanExecuteChanged();
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
                RaisePropertyChanged(nameof(ContactName));
                RaisePropertyChanged(nameof(ContactEmail));

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
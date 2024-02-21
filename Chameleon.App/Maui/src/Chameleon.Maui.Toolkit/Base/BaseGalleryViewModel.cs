using Chameleon.Interfaces.Services;
using Chameleon.Maui.Toolkit.Models;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics.CodeAnalysis;

namespace Chameleon.Maui.Toolkit.Base;
public abstract partial class BaseGalleryViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    protected BaseGalleryViewModel(SectionModel[] items, INavigationService navigationService) : base(navigationService)
    {
        _navigationService = navigationService;

        if (DoesItemsArrayContainDuplicates(items, out var duplicatedSectionModels))
        {
            throw new InvalidOperationException($"Duplicate {nameof(SectionModel)}.{nameof(SectionModel.ViewModelType)} found for {duplicatedSectionModels[0].ViewModelType}");
        }

        Items = items.OrderBy(x => x.Title).ToList();
    }

    public IReadOnlyList<SectionModel> Items { get; }

    static bool DoesItemsArrayContainDuplicates(in SectionModel[] items, [NotNullWhen(true)] out IReadOnlyList<SectionModel>? duplicatedSectionModels)
    {
        var discoveredDuplicatedSectionModels = new List<SectionModel>();

        var itemsGroupedByViewModelType = items.GroupBy(x => x.ViewModelType);
        foreach (var duplicatedItemsGroups in itemsGroupedByViewModelType.Where(x => x.Count() > 1))
        {
            discoveredDuplicatedSectionModels.AddRange(duplicatedItemsGroups);
        }

        if (discoveredDuplicatedSectionModels.Any())
        {
            duplicatedSectionModels = discoveredDuplicatedSectionModels;
            return true;
        }
        else
        {
            duplicatedSectionModels = null;
            return false;
        }
    }

    [RelayCommand]
    private async Task NagiateAsync(SectionModel selection)
    {
        await _navigationService.NavigateToAsync(selection.ViewModelType);
    }
}

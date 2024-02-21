using Chameleon.Maui.Toolkit.Base;
using Chameleon.Core.Extensions;

namespace Chameleon.Maui.Toolkit.Helpers;
public sealed class PageViewModelRouting
{
    static PageViewModelRouting() { }                           
    private PageViewModelRouting() { }

    public static PageViewModelRouting Instance { get; } = new PageViewModelRouting();

    public Dictionary<Type, (Type GalleryPageType, Type ContentPageType)> ViewModelMappings { get; } = new Dictionary<Type, (Type, Type)>();
    public void AddMappings(IEnumerable<KeyValuePair<Type, (Type GalleryPageType, Type ContentPageType)>> mappingsToAdd)
    {
        ViewModelMappings.AddValuesRange(mappingsToAdd);
    }
    public string GetPageRoute<TViewModel>() where TViewModel : BaseViewModel
    {
        return GetPageRoute(typeof(TViewModel));
    }

    public string GetPageRoute(Type viewModelType)
    {
        if (!viewModelType.IsAssignableTo(typeof(BaseViewModel)))
        {
            throw new ArgumentException($"{nameof(viewModelType)} must implement {nameof(BaseViewModel)}", nameof(viewModelType));
        }

        if (!ViewModelMappings.TryGetValue(viewModelType, out (Type GalleryPageType, Type ContentPageType) mapping))
        {
            throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings. Please register your ViewModel in {nameof(Routing)}.{nameof(ViewModelMappings)}");
        }

        var uri = new UriBuilder("", GetPageRoute(mapping.GalleryPageType, mapping.ContentPageType));
        return uri.Uri.OriginalString[..^1];
    }

    string GetPageRoute(Type galleryPageType, Type contentPageType) => $"//{galleryPageType.Name}/{contentPageType.Name}";

    public static KeyValuePair<Type, (Type GalleryPageType, Type ContentPageType)> CreateViewModelMapping<TPage, TViewModel, TGalleryPage, TGalleryViewModel>() where TPage : BasePage<TViewModel>
                                                                                                                                                            where TViewModel : BaseViewModel
                                                                                                                                                            where TGalleryPage : BaseGalleryPage<TGalleryViewModel>
                                                                                                                                                            where TGalleryViewModel : BaseGalleryViewModel
    {
        return new KeyValuePair<Type, (Type GalleryPageType, Type ContentPageType)>(typeof(TViewModel), (typeof(TGalleryPage), typeof(TPage)));
    }


}

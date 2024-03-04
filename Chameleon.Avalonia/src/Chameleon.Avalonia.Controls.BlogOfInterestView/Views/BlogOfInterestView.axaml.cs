using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Prospector;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.BlogOfInterestView;

public partial class BlogOfInterestView : UserControl
        , IBlogOfInterestView
{
    public BlogOfInterestView()
    {
        InitializeComponent();
    }

    public IUserProfile UserProfile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
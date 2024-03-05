namespace Chameleon.Avalonia.Controls.Paginator.ViewModels;

public class PaginatorButtonViewModel
{
    public int Index { get; }
    public string Text => (Index + 1).ToString();

    public PaginatorButtonViewModel(int index)
    {
        Index = index;
    }


}

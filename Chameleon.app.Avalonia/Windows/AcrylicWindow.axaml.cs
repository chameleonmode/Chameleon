using Avalonia.Controls;
using Avalonia.Input;

namespace Chameleon.app.Avalonia.Windows;

public partial class AcrylicWindow : Window {
	public AcrylicWindow()
	{
		InitializeComponent();
		CloseBtn.Click += CloseBtn_Click;
	}

	private void CloseBtn_Click(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
	{
		this.Close();
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		base.OnPointerMoved(e);
		if (e.Pointer.Captured != null) {
			//var currentPosition = e.GetPosition(this);

			//var offsetX = currentPosition.X - _positionInBlock.X;
			//var offsetY = currentPosition.Y - _positionInBlock.Y;

			//var _transform = new TranslateTransform(offsetX, offsetY);
			//RenderTransform = _transform;
			// this.BeginMoveDrag(new PointerPressedEventArgs(e.Source, e.Pointer, this, e.GetPosition(this),e.Timestamp,e.p,));

		}
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		base.OnPointerPressed(e);
		if (e.Source != null && e.Source is Panel)
			BeginMoveDrag(e);
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		base.OnPointerReleased(e);
	}

	protected override void OnPointerEntered(PointerEventArgs e)
	{
		base.OnPointerEntered(e);
		this.Opacity = 1;
	}

	protected override void OnPointerExited(PointerEventArgs e)
	{
		base.OnPointerExited(e);
		this.Opacity = 0.4;
	}
}
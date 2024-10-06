using System.Xml.Linq;

using Avalonia.Controls;

using chameleon.assets;

using Chameleon.Av.Fluent.Dialogs.ViewModels;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.Dialogs;
using Chameleon.Interfaces.Dialogs.ViewModels;
using Chameleon.Interfaces.Dialogs.Views;
using Chameleon.Interfaces.Services;
using Chameleon.lib.Common.Constants;
using Chameleon.lib.Common.Interfaces.Services;
using Chameleon.lib.Common.Interfaces.Systemics;

using FluentAvalonia.UI.Controls;

namespace Chameleon.Av.Fluent.Dialogs.Services;

public class ContentDialogService : IContentDialogService {
	async Task<IContentDialogResult> CreateDialog(IContentDialogAware contentDialog, Action<IContentDialogResult> OnClosing = null)
	{
		var dialog = new ContentDialog() {
			Title = contentDialog.Title,
			Content = contentDialog.DialogContent,
			PrimaryButtonText = contentDialog.PrimaryButtonText,
			SecondaryButtonText = contentDialog.SecondaryButtonText,
			CloseButtonText = contentDialog.CloseButtonText,
			DefaultButton = ContentDialogButton.Primary,
		};
		if (OnClosing != null) {
			dialog.Closing += (s, e) => {
				OnClosing?.Invoke((IContentDialogResult)e.Result);
			};
		}
		var res = await dialog.ShowAsync(ApplicationHelper.GetMainWindow());
		return (IContentDialogResult)res;
	}
	public Task<IContentDialogResult> ShowAsync<TView, TViewModel>(Action<TViewModel> initialize) where TViewModel : class
	{
		if (ContainerServiceHelper.Resolve<TView>() is Control view) {
			var viewModel = view.DataContext as TViewModel;

			initialize?.Invoke(viewModel);

			string? title = null;
			if (viewModel is IPageViewModel pvm)
				title = pvm.Title;

			return CreateDialog(
					new DefaultContentDialogView(ContentDialogButtons.OKCancel, view, title),
					viewModel is IContentDialogViewModel cdvm ? cdvm.OnDialogClosing : null);


		}

		throw new ArgumentNullException("TView");
	}
	public async Task<IContentDialogResult> ShowContentDialogAsync(object content, Action<IContentDialogResult> OnClosing, string title = "False", ContentDialogButtons btns = ContentDialogButtons.OKCancel)
	{
		return await CreateDialog(new DefaultContentDialogView(btns, content, title), OnClosing);
	}

	public async Task<IContentDialogResult> ShowContentDialogAsync(IContentDialogAware contentDialog)
	{
		return await CreateDialog(contentDialog);
	}

	public async Task<IContentDialogResult> ShowContentDialogAsync(Type contentDialog)
	{
		var c = ContainerServiceHelper.Current.ContainerProvider?.Resolve<IContentDialogView>(contentDialog);
		var dialog = new ContentDialog() {
			Title = c.Title,
			Content = c,
			PrimaryButtonText = c.PrimaryButtonText,
			SecondaryButtonText = c.SecondaryButtonText,
			CloseButtonText = c.CloseButtonText,
			DefaultButton = ContentDialogButton.Primary,
		};

		var res = await dialog.ShowAsync();
		return (IContentDialogResult)res;
	}


	public async Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content,
			object? title = null,
			string? primaryBtnTxt = null,
			string? secondaryBtnTxt = null,
			string? closebtnTxt = null)
	{
		return await ShowContentDialogAsync(new DefaultContentDialogView(btns, content, title, primaryBtnTxt, secondaryBtnTxt, closebtnTxt));
	}
	public async Task<IContentDialogResult> ShowContentDialogAsync(ContentDialogButtons btns, object content, object title)
	{
		return await ShowContentDialogAsync(btns, content, title,
				null,
				null,
				null);
	}
	public async Task ShowContentDialogAsync(object content,
			object? title = null,
			Action? action = null,
			IContentDialogResult onResult = IContentDialogResult.Primary,
			ContentDialogButtons btns = ContentDialogButtons.YesNo,
			string? primaryBtnTxt = null,
			string? secondaryBtnTxt = null,
			string? closebtnTxt = null)
	{
		if (await ShowContentDialogAsync(btns, content, title, primaryBtnTxt, secondaryBtnTxt, closebtnTxt) == onResult)
			action?.Invoke();
	}

	public async Task<IContentDialogResult> ShowContentDialogAsync(string title, string content, ContentDialogButtons btns = ContentDialogButtons.YesNo, IFontIconInfo? fontIconInfo = null)
	{
		var c = ContainerServiceHelper.Resolve<IDefaultContentDialogContentViewModel>();
		if (c == null)
			return IContentDialogResult.None;

		c.Title = title;
		c.Glyph = fontIconInfo?.Glyph;

		var dialog = new ContentDialog() {
			Title = ContainerServiceHelper.Resolve<IDefaultContentDialogTitle>(),
			Content = content,
			DataContext = c,
			PrimaryButtonText = DefaultContentDialogView.GetPrimaryButtonText(btns),
			SecondaryButtonText = DefaultContentDialogView.GetSecondaryButtonText(btns),
			CloseButtonText = DefaultContentDialogView.GetCloseButtonText(btns),
			DefaultButton = ContentDialogButton.Primary,
		};
		var res = await dialog.ShowAsync();
		return (IContentDialogResult)res;
	}
}
using Chameleon.app.Avalonia.Models;
using DynamicData.Binding;

namespace Chameleon.app.Avalonia.Com.DynamicData;
public static class Compares {
	public static class UserProfileVimCompares {
		public static SortExpressionComparer<UserProfileVim> AscendingComparer => SortExpressionComparer<UserProfileVim>.Ascending(p => p.Dto!.title!);
		public static SortExpressionComparer<UserProfileVim> DescendingComparer => SortExpressionComparer<UserProfileVim>.Descending(p => p.Dto!.title!);
	}
	public static class FolderVimCompares {
		public static SortExpressionComparer<FolderVim> AscendingComparer => SortExpressionComparer<FolderVim>.Ascending(p => p.Dto!.title!);
		public static SortExpressionComparer<FolderVim> DescendingComparer => SortExpressionComparer<FolderVim>.Descending(p => p.Dto!.title!);
	}
}

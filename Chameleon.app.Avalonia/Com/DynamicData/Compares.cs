using Chameleon.app.Avalonia.Models.Observable;

using DynamicData.Binding;

namespace Chameleon.app.Avalonia.Com.DynamicData;
public static class Compares {
	public static class ObsProfileCompares {
		public static SortExpressionComparer<ObsProfile> AscendingComparer => SortExpressionComparer<ObsProfile>.Ascending(p => p.Dto!.title!);
		public static SortExpressionComparer<ObsProfile> DescendingComparer => SortExpressionComparer<ObsProfile>.Descending(p => p.Dto!.title!);
	}
	public static class ObsFolderCompares {
		public static SortExpressionComparer<ObsFolder> AscendingComparer => SortExpressionComparer<ObsFolder>.Ascending(p => p.Dto!.title!);
		public static SortExpressionComparer<ObsFolder> DescendingComparer => SortExpressionComparer<ObsFolder>.Descending(p => p.Dto!.title!);
	}
	public static class ObsProxySettingCompares {
		public static SortExpressionComparer<ObsProxySetting> AscendingComparer => SortExpressionComparer<ObsProxySetting>.Descending(p => p.ObsProfile.IsSelected).ThenByAscending(p => p.ObsProfile.Title!);
		public static SortExpressionComparer<ObsProxySetting> DescendingComparer => SortExpressionComparer<ObsProxySetting>.Descending(p => p.ObsProfile.Title!);
	}
}

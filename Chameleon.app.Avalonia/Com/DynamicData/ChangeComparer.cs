using DynamicData.Binding;
using DynamicData;
using System.Reactive.Linq;
using Chameleon.lib.Common.Constants;

namespace Chameleon.app.Avalonia.Com.DynamicData;

public sealed class ChangeComparer : AbstractNotifyPropertyChanged, IDisposable {
	private Enums.ChangeComparereOption _option;
	private readonly IDisposable _cleanUp;

	public IObservableList<int> DataSource { get; }

	public ChangeComparer(IObservableList<int> source)
	{
		/*
		 * Pass IObservable<IComparer<T>> into the sort operator to switch sorting
		 * 
		 * The same concept applies to the ObservableCache
		 */

		var optionChanged = this.WhenValueChanged(@this => @this.Option)
				.Select(opt => opt == Enums.ChangeComparereOption.Ascending
						? SortExpressionComparer<int>.Ascending(i => i)
						: SortExpressionComparer<int>.Descending(i => i));

		//create a sorted observable list
		DataSource = source.Connect()
										.Sort(optionChanged)
										.AsObservableList();

		_cleanUp = DataSource;
	}

	public Enums.ChangeComparereOption Option {
		get => _option;
		set => SetAndRaise(ref _option, value);
	}

	public void Dispose()
	{
		_cleanUp.Dispose();
	}
}

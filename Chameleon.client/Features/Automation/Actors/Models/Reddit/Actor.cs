using System.Collections.ObjectModel;
using Chameleon.lib.Playwright.Interfaces;
using Chameleon.lib.Playwright.Scripts.JS.Reddit.Post;
using Chameleon.lib.Playwright.Scripts.JS.Reddit.Subreddit;

namespace Chameleon.client.Features.Automation.Actors.Models.Reddit;

public class Actor : IActor {
	public IOptions Options { get; set; } = new Options();
	public IEnumerable<IBundledJSScript> PlaywrightScripts { get; set; } = new ObservableCollection<IBundledJSScript>() {
    new Comment(),
		new Reply(),
		new Join(),
		new Post(),
		new Vote(),
  };
}

using Chameleon.lib.Api;
using Chameleon.lib.Auth;
using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.WebBrowser.Services;

namespace Chameleon.Common;

public class Project {
  public static async Task<bool> Logineer(LoginSettings login) {
    Session.I.Auth0Client.OidcBrowser.Open = async url => {
      var browser = await EX.Catch(
        async () => await SystemBrowser.I.Open(Factorially.Chrome(url)),
        ex => { if (!BrowserInfo.Find(BrowserType.Chrome).Exists) ProcessUtil.OpenBrowser(url); }
      );
      _ = Session.I.Auth0Client.OidcBrowser.TaskCompletion?.Task.ContinueWith(_ => browser?.Closee());
    };
    await Session.I.Login(login);
    await Auther.LoginAsync(login.LoginName, login.LicenseKey);
    return Auther.AuthSession is not null ? true : throw new InvalidOperationException("Auth session is invalid after login");
  }
}

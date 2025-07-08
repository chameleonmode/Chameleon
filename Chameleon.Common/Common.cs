using Chameleon.lib.Api;
using Chameleon.lib.Auth;
using Chameleon.lib.Util;
using Chameleon.lib.WebBrowser;
using Chameleon.lib.WebBrowser.Browsers;
using Chameleon.lib.WebBrowser.Services;

namespace Chameleon.Common;

public class Project {
  public static async Task<bool> Logineer(LoginSettings login) {
    IBrowserInstance? browser = null;
    Session.I.OpenBrowser = async url => {
      browser = await EX.Catch(
        async () => await SystemBrowser.I.Open(Factorially.Chrome(url)),
        ex => ProcessUtil.OpenBrowser(url)
      );
    };
    await Session.I.Login(login);
    await Task.Delay(500); 
    _ = browser?.Closee();

    await Auther.LoginAsync(login.LoginName, login.LicenseKey);
    return Auther.AuthSession is not null ? true : throw new InvalidOperationException("Auth session is invalid after login");
  }
}

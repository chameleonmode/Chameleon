using System.IO;

namespace Chameleon.SystemBrowser.Firefox;
public class FirefoxSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        ISystemBrowserInfoManager systemBrowserInfoManager,
        IUserDefaultSettingsService userDefaultsSettingsService) : SystemBrowserBase(eventAggregator), IFirefoxSystemBrowser
{
    public const string FirefoxChameleonDirectory = "FirefoxChameleon";

    readonly string directoryForCopy = IsMao ?
    System.IO.Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app")
    : System.IO.Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory);

    string Path => GetSystemBrowserExePath();
    string ChamelonPath => GetBrowserExePath();

    string Directory => IsMao ?
    "Applications/firefox.app"
    : System.IO.Path.GetDirectoryName(Path);


    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        return new FirefoxSystemBrowserInstance(
            EventAggregator,
            o,
            userDefaultsSettingsService,
            applicationEnvironment.ApplicationDataFolderPath,
            GetBrowserExePath());
    }

    public override async Task<ISystemBrowserInstance> InitializeBrowserAsync(ISystemBrowserLaunchOptions o)
    {
        await CreateChameleonFirefoxCopy();

        return InitializeBrowser(o);
    }

    private async Task CreateChameleonFirefoxCopy()
    {
        if (IOtil.IsNeedUpdate(Path, ChamelonPath))
        {
            await IOtil.DeleteDExistsAsync(directoryForCopy);

            await IOtil.CopyFolderAsync(Directory, directoryForCopy);

            await Task.Delay(1000);
        }


        await AddAutoloadTemporaryAddon(System.IO.Path.Combine(directoryForCopy));
    }

    private async Task AddAutoloadTemporaryAddon(string directory)
    {
        var browserExtensionsFolderPath = AddonsUtil.BERFFF;

        var userChrome = $@"
 // First line is always a comment
 lockPref(""a.b.c.d"", ""1.2.3.4""); // Debugging Firefox AutoConfig Problems

 function reportError(ex) {{
     Components.utils.reportError(""userChrome.js Ex("" + ex + "")"");
 }}

 function printDebug(text) {{
     Components.utils.reportError(""userChrome.js "" + text);
 }}

 // Based on class Addon {{ static async install(path, temporary = false) ... }}
 // d:\Files\Firefox102.2.0esr\omni_ja\chrome\remote\content\marionette\addon.js
 // from https://developer.mozilla.org/en-US/Add-ons/Add-on_Manager/AddonManager#AddonInstall_errors
 const ERRORS = {{
   [-1]: ""ERROR_NETWORK_FAILURE: A network error occurred."",
   [-2]: ""ERROR_INCORRECT_HASH: The downloaded file did not match the expected hash."",
   [-3]: ""ERROR_CORRUPT_FILE: The file appears to be corrupt."",
   [-4]: ""ERROR_FILE_ACCESS: There was an error accessing the filesystem."",
   [-5]: ""ERROR_SIGNEDSTATE_REQUIRED: The addon must be signed and isn't."",
 }};

 async function installAddon(file) {{
     let install = await AddonManager.getInstallForFile(file, null, {{ source: ""internal"", }});
     if (install.error) {{
         reportError(ERRORS[install.error]);
     }}
     return install.install().catch(err => {{
         reportError(ERRORS[install.error]);
     }});
 }}

 async function installExtension(path, temporary) {{
     let addon;
     let file;

     printDebug(""installTemporaryExtension("" + path + "")"");
     try {{
       file = new FileUtils.File(path);
     }} catch (ex) {{
         reportError(`Expected absolute path: ${{ex}}`, ex);
     }}

     if (!file.exists()) {{
         reportError(`No such file or directory: ${{path}}`);
     }}

     try {{
         if (temporary) {{
             addon = await AddonManager.installTemporaryAddon(file);
         }} else {{
             addon = await installAddon(file);
         }}
     }} catch (ex) {{
         reportError(`Could not install add-on: ${{path}}: ${{ex.message}}`, ex);
     }}
 }}

 async function installUnpackedExtensions() {{
     var folder = Services.dirsvc.get(""ProfD"", Ci.nsIFile).path;
     const BrowserExtensionsFolderPath = ""{browserExtensionsFolderPath}"";

     const GetinstallExtension = {(IsMao ?
              "await installExtension(`${folder}/ChameleonAutoExt/autoproxy.chameleon.zip`, true);" :
              "await installExtension(`${folder}\\\\ChameleonAutoExt\\\\autoproxy.chameleon.zip`, true);")}

     await GetinstallExtension;

     let iterator = new OS.File.DirectoryIterator(BrowserExtensionsFolderPath);
     try {{
         await iterator.forEach(async function(entry) {{
             if (entry.name.endsWith('.xpi')) {{
                 printDebug(`Attempting to install: ${{entry.name}}`);
                 await installExtension(entry.path, true);
             }}
         }});
     }} finally {{
         iterator.close();
     }}

     await setPermission(""autoproxy@chameleonmode.com"");
 }}

 async function setPermission(addonId) {{
     const PRIVATE_BROWSING_PERMS = {{
         permissions: [""internal:privateBrowsingAllowed""],
         origins: [],
     }};

     const {{ExtensionPermissions}} = ChromeUtils.import(""resource://gre/modules/ExtensionPermissions.jsm"");

     const myaddons = await AddonManager.getAddonsByTypes([""extension""]);
     for(let addon of myaddons){{
         if (addon.id !== addonId){{
             continue;
         }}

         await ExtensionPermissions.add(addon.id, PRIVATE_BROWSING_PERMS);
         if (addon.isActive)
             addon.reload();
     }}
 }}

 try {{
   let {{ classes: Cc, interfaces: Ci, manager: Cm  }} = Components;

   function ConfigJS() {{
       Services.obs.addObserver(this, 'final-ui-startup', false);
   }}

   const {{ AddonManager }} =
       Components.utils.import(""resource://gre/modules/AddonManager.jsm"");

   const {{ FileUtils }} =
       Components.utils.import(""resource://gre/modules/FileUtils.jsm"");

   ConfigJS.prototype = {{
       observe: async function observe(subject, topic, data) {{
           switch (topic) {{
               case 'final-ui-startup':
                   await installUnpackedExtensions();
                   break;
           }}
       }}
   }};

   if (!Services.appinfo.inSafeMode) {{
       new ConfigJS();
   }}

 }} catch(ex) {{
     reportError(ex);
 }};

 lockPref(""e.f.g.h"", ""5.6.7.8""); // Debugging Firefox AutoConfig Problems
 ";

        var configPrefs = @"
// config-prefs.js file for [Firefox program folder]\defaults\pref
pref(""general.config.obscure_value"", 0);
// the file named in the following line must be in [Firefox program folder]
pref(""general.config.filename"", ""userChrome.js"");
// disable the sandbox to run unsafe code
pref(""general.config.sandbox_enabled"", false);
";

        var ucp = System.IO.Path.Combine(directory, "Contents", "Resources", "userChrome.js");
        var cpp = System.IO.Path.Combine(directory, "Contents", "Resources", "defaults", "pref", "config-prefs.js");
        if (!IsMao)
        {
            ucp = System.IO.Path.Combine(directory, "userChrome.js");
            cpp = System.IO.Path.Combine(directory, "defaults", "pref", "config-prefs.js");
        }
        await File.WriteAllTextAsync(ucp, userChrome);
        await File.WriteAllTextAsync(cpp, configPrefs);
    }

    private string GetSystemBrowserExePath()
    {
        return systemBrowserInfoManager
            .FindByName("firefox")
            .Path;
    }

    private string GetBrowserExePath()
    {
        string path = OperatingSystem.IsMacOS()
            ? System.IO.Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app", "Contents", "MacOS", "firefox")
            : System.IO.Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.exe");

        return path;
    }
}


namespace Chameleon.SystemBrowser.Addons;

public static class AddonsUtil
{
    public static bool IMac => OperatingSystem.IsMacOS();
    // public static string BrowserExtensionsRootFolderPath => IMac ?
    // "/Applications/Chameleon.app/Contents/Resources/BrowserExtensions/mac"
    // : Path.Combine(Directory.GetCurrentDirectory(), "BrowserExtensions");
    public static string BrowserExtensionsRootFolderPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BrowserExtensions");

    public static string BERFFF => 
        Path.Combine(BrowserExtensionsRootFolderPath, "firefox");

    public static async Task AddAutoloadTemporaryAddonFF(string directory)
    {
        var browserExtensionsFolderPath = AddonsUtil.BERFFF.Replace("\\", "\\\\");

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
    const BrowserExtensionsFolderPath = `{browserExtensionsFolderPath}`;

    const {{ FileUtils }} = ChromeUtils.import(""resource://gre/modules/FileUtils.jsm"", {{}});
    printDebug(`BrowserExtensionsFolderPath: ${{BrowserExtensionsFolderPath}}`);
    
    let dir;
    try {{
        dir = new FileUtils.File(BrowserExtensionsFolderPath);
        if (!dir.exists() || !dir.isDirectory()) {{
            throw new Error(""Directory not found or is not a directory"");
        }}
    }} catch (ex) {{
        reportError(`Directory not found: ${{BrowserExtensionsFolderPath}}`);
    }}

    try {{
        let entries = dir.directoryEntries;
        while (entries.hasMoreElements()) {{
            let entry = entries.getNext().QueryInterface(Ci.nsIFile);
            if (entry.isFile() && entry.leafName.endsWith('.xpi')) {{
                printDebug(`Attempting to install: ${{entry.leafName}}`);
                await installExtension(entry.path, true);
            }}
        }}
    }} catch (ex) {{
        reportError(`Error iterating directory: ${{ex.message}}`);
    }}


     var folder = Services.dirsvc.get(""ProfD"", Ci.nsIFile).path;
     {(IMac ?
              "await installExtension(`${folder}/ChameleonAutoExt/autoproxy.chameleon.zip`, true);" :
              "await installExtension(`${folder}\\\\ChameleonAutoExt\\\\autoproxy.chameleon.zip`, true);")}


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

        var ucp = IMac ? Path.Combine(directory, "Contents", "Resources", "userChrome.js") : Path.Combine(directory, "userChrome.js");
        var cpp = IMac ? Path.Combine(directory, "Contents", "Resources", "defaults", "pref", "config-prefs.js") : Path.Combine(directory, "defaults", "pref", "config-prefs.js");
        await File.WriteAllTextAsync(ucp, userChrome);
        await File.WriteAllTextAsync(cpp, configPrefs);
    }
}

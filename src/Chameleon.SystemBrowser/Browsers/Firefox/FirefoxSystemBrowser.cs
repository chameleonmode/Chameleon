namespace Chameleon.SystemBrowser.Firefox;
public class FirefoxSystemBrowser(
        IEventAggregator eventAggregator,
        IApplicationEnvironment applicationEnvironment,
        IUserDefaultSettingsService userDefaultsSettingsService) : SystemBrowserBase, IFirefoxSystemBrowser
{

    public const string FirefoxChameleonDirectory = "FirefoxChameleon";

    public override ISystemBrowserInstance InitializeBrowser(ISystemBrowserLaunchOptions o)
    {
        CreateChameleonFirefoxCopy();

        return new FirefoxSystemBrowserInstance(
            eventAggregator,
            o,
            userDefaultsSettingsService,
            applicationEnvironment.ApplicationDataFolderPath,
            GetBrowserExePath());
    }

    private void CreateChameleonFirefoxCopy()
    {
        string path = GetSystemBrowserExePath();
        string chamelonPath = GetBrowserExePath();

        if (!IsNeedUpdate(path, chamelonPath))
        {
            return;
        }

        string directory = IsMao ? "Applications/firefox.app" : Path.GetDirectoryName(path);
        string directoryForCopy = IsMao ? Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app")
        : Path.Combine(applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory);


        IOtil.DeleteDExists(directoryForCopy);

        CopyFolder(directory, directoryForCopy);
        AddAutoloadTemporaryAddon(Path.Combine(directoryForCopy));
    }

    private bool IsNeedUpdate(string systemFirefox, string chamelonFirefox)
    {
        if (!Path.Exists(chamelonFirefox))
        {
            return true;
        }

        FileVersionInfo systemFirefoxInfo = FileVersionInfo.GetVersionInfo(systemFirefox);
        FileVersionInfo chamelonFirefoxInfo = FileVersionInfo.GetVersionInfo(chamelonFirefox);

        bool isEqual = chamelonFirefoxInfo.ProductMajorPart == systemFirefoxInfo.ProductMajorPart
            && chamelonFirefoxInfo.ProductMinorPart == systemFirefoxInfo.ProductMinorPart;

        return !isEqual;
    }

    private void CopyFolder(string directory, string directoryForCopy)
    {
        Directory.CreateDirectory(directoryForCopy);

        string[] filePaths = Directory.GetFiles(directory);
        foreach (string filePath in filePaths)
        {
            string fileName = Path.GetFileName(filePath);
            string newFile = Path.Combine(directoryForCopy, fileName);

            File.Copy(filePath, newFile);
        }

        string[] subdirectoryPaths = Directory.GetDirectories(directory);
        foreach (string subdirectory in subdirectoryPaths)
        {
            string subdirectoryName = Path.GetFileName(subdirectory);
            string newSubdirectory = Path.Combine(directoryForCopy, subdirectoryName);

            CopyFolder(subdirectory, newSubdirectory);
        }
    }

    private void AddAutoloadTemporaryAddon(string directory)
    {
        var GetinstallExtension = IsMao ?
        "await installExtension(`${folder}/ChameleonAutoExt/autoproxy.chameleon.zip`, true);"
        : "await installExtension(`${folder}\\\\ChameleonAutoExt\\\\autoproxy.chameleon.zip`, true);"
;
        string userChrome = """ 
                    // First line is always a comment
                    lockPref("a.b.c.d", "1.2.3.4"); // Debugging Firefox AutoConfig Problems

                    function reportError(ex) {
                        Components.utils.reportError("userChrome.js Ex(" + ex + ")");
                    }

                    function printDebut(text) {
                    	Components.utils.reportError("userChrome.js " + text);
                    }

                    // Based on class Addon { static async install(path, temporary = false) ... }
                    // d:\Files\Firefox102.2.0esr\omni_ja\chrome\remote\content\marionette\addon.js
                    // from https://developer.mozilla.org/en-US/Add-ons/Add-on_Manager/AddonManager#AddonInstall_errors
                    const ERRORS = {
                      [-1]: "ERROR_NETWORK_FAILURE: A network error occured.",
                      [-2]: "ERROR_INCORECT_HASH: The downloaded file did not match the expected hash.",
                      [-3]: "ERROR_CORRUPT_FILE: The file appears to be corrupt.",
                      [-4]: "ERROR_FILE_ACCESS: There was an error accessing the filesystem.",
                      [-5]: "ERROR_SIGNEDSTATE_REQUIRED: The addon must be signed and isn't.",
                    };

                    // Untested...
                    async function installAddon(file) {
                    	let install = await AddonManager.getInstallForFile(file, null,
                    		{ source: "internal", });
                    	if (install.error) {
                    		reportError(ERRORS[install.error]);
                    	}
                    	return install.install().catch(err => {
                    		reportError(ERRORS[install.error]);
                    	});
                    }

                    async function installExtension(path, temporary) {
                        let addon;
                        let file;

                    	printDebut("installTemporaryExtension(" + path + ")");
                        try {
                          file = new FileUtils.File(path);
                        } catch (ex) {
                    		reportError(`Expected absolute path: ${ex}`, ex);
                        }

                        if (!file.exists()) {
                    		reportError(`No such file or directory: ${path}`);
                        }

                        try {
                    		if (temporary) {
                    			addon = await AddonManager.installTemporaryAddon(file);
                    		} else {
                    			addon = installAddon(file);
                    		}
                        } catch (ex) {
                    		reportError(`Could not install add-on: ${path}: ${ex.message}`, ex);
                        }
                    }

                    async function installUnpackedExtensions() {
                        var folder = Services.dirsvc.get("ProfD", Ci.nsIFile).path; 

                    """ +
                $"{GetinstallExtension}"
                + """
                    
                    await setPermission("autoproxy@chameleonmode.com");
                }

                async function setPermission(addonId) {
                    const PRIVATE_BROWSING_PERMS = {
                        permissions: ["internal:privateBrowsingAllowed"],
                        origins: [],
                    };

                    const {ExtensionPermissions} = ChromeUtils.import("resource://gre/modules/ExtensionPermissions.jsm");

                	const myaddons = await AddonManager.getAddonsByTypes(["extension"]);
                    for(let addon of myaddons){
                		if (addon.id !== addonId){
                			continue;
                		}

                        await ExtensionPermissions.add(addon.id, PRIVATE_BROWSING_PERMS);
                        if (addon.isActive)
                            addon.reload();
                    }
                }

                try {
                  let { classes: Cc, interfaces: Ci, manager: Cm  } = Components;

                  function ConfigJS() {
                	  Services.obs.addObserver(this, 'final-ui-startup', false);
                  }

                  const { AddonManager } =
                	  Components.utils.import("resource://gre/modules/AddonManager.jsm");

                  const { FileUtils } =
                	  Components.utils.import("resource://gre/modules/FileUtils.jsm");

                  ConfigJS.prototype = {

                	  observe: async function observe(subject, topic, data) {
                		  switch (topic) {
                			  case 'final-ui-startup':
                			  installUnpackedExtensions(); 
                			  break;
                		  }
                	  }
                };


                  if (!Services.appinfo.inSafeMode) {
                	  new ConfigJS();
                  }

                } catch(ex) {
                	reportError(ex);
                };

                lockPref("e.f.g.h", "5.6.7.8"); // Debugging Firefox AutoConfig Problems
                """;

        string configPrefs = """
                // config-prefs.js file for [Firefox program folder]\defaults\pref
                pref("general.config.obscure_value", 0);
                // the file named in the following line must be in [Firefox program folder]
                pref("general.config.filename", "userChrome.js");
                // disable the sandbox to run unsafe code
                pref("general.config.sandbox_enabled", false);
                """;

        var ucp = Path.Combine(directory, "Contents", "Resources",
        "userChrome.js");
        var cpp = Path.Combine(directory, "Contents", "Resources", "defaults", "pref",
        "config-prefs.js");
        if (!IsMao)
        {
            ucp = Path.Combine(directory, "userChrome.js");
            cpp = Path.Combine(directory, "defaults", "pref", "config-prefs.js");
        }
        File.WriteAllText(ucp, userChrome);
        File.WriteAllText(cpp, configPrefs);
    }

    private string GetSystemBrowserExePath()
    {
        return _systemBrowserInfoManager
            .FindByName("firefox")
            .Path;
    }

    private string GetBrowserExePath()
    {
        string path = OperatingSystem.IsMacOS()
            ? Path.Combine(_applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.app", "Contents", "MacOS", "firefox")
            : Path.Combine(_applicationEnvironment.ApplicationDataFolderPath, FirefoxChameleonDirectory, "firefox.exe");

        return path;
    }
}

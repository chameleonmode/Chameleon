using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.SystemBrowser.Addons;
public class ExtensionDirectory
{
    public string MainDir { get; }
    public string AddonDir { get; }
    public string AddonFolderName { get; }


    public ExtensionDirectory(string browserProfileAddonsDir, string addonFolderName)
    {
        MainDir = browserProfileAddonsDir;
        AddonFolderName = addonFolderName;
        AddonDir = Path.Combine(browserProfileAddonsDir, addonFolderName);
    }
}

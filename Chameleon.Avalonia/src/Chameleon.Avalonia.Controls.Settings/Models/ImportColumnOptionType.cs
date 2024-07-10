using System.ComponentModel;

namespace Chameleon.Controls.ImportExport.Models
{
    public enum ImportColumnOptionType
    {
        [Description("None")]
        None,

        [Description("Profile Name")]
        ProfileName,

        [Description("Email")]
        Email,

        [Description("Full Name")]
        FullName,

        [Description("First Name")]
        FirstName,

        [Description("Middle Name")]
        MiddleName,

        [Description("Last Name")]
        LastName,

        [Description("Date of Birth")]
        BirthDate,

        [Description("Day of Birth")]
        BirthDay,

        [Description("Month of Birth")]
        BirthMonth,

        [Description("Year of Birth")]
        BirthYear,

        [Description("Street")]
        Street,

        [Description("City")]
        City,

        [Description("State Code")]
        State,

        [Description("ZIP Code")]
        Zip,

        [Description("Country")]
        Country,

        [Description("Proxy Host/Ip")]
        ProxyHost,

        [Description("Proxy Port")]
        ProxyPort,

        [Description("Proxy Username")]
        ProxyUserName,

        [Description("Proxy Password")]
        ProxyPassword,

        [Description("Login Title")]
        LoginTitle,

        [Description("Login Web Site")]
        LoginWebSite,

        [Description("Login Email")]
        LoginEmail,

        [Description("Login Username")]
        LoginUserName,

        [Description("Login Password")]
        LoginPassword,

        [Description("Login Notes")]
        LoginNotes,

        [Description("Phone Number")]
        PhoneNumber,
    }
}

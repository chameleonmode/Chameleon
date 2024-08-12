using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.ThirdParty;

namespace Chameleon.Interfaces.App.Settings;

public interface IPhoneVerificationViewModel
    : IPageViewModel,
    ISingletonDependency
{
    IPVApiModel CodesVerify { get; }
    IPVApiModel SMSPVA { get; }
}

public interface IPVApiModel
    : IPageViewModel
{
    string ApiKey { get; set; }

    string GetNumberData { get; set; }
    string ReceiveSMSData { get; set; }
    string LastFormatedResponse { get; set; }

    bool IsVisible { get; set; }
    bool IsVisibleSave { get; set; }
    bool IsAwaiting {  get; set; }
    bool CanCancel {  get; set; }

    IList<RCountry> Countries { get; set; }
    RCountry SelectedCountry { get; set; }
    IList<RService> Apps { get; set; }
    RService SelectedApp { get; set; }
}
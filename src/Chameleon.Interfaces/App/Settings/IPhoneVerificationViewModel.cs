using Chameleon.Interfaces.Ioc;

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
}
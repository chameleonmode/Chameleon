using System.Threading.Tasks;
using Chameleon.Models.TokenAuth;
using Chameleon.Web.Controllers;
using Shouldly;
using Xunit;

namespace Chameleon.Web.Tests.Controllers
{
    public class HomeController_Tests: ChameleonWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}
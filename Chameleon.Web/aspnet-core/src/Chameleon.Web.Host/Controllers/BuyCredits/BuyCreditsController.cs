using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Web.Models;
using Chameleon.App;
using Chameleon.App.Entities;
using Chameleon.Authorization.Users;
using Chameleon.Controllers;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chameleon.Web.Host.Controllers
{
    public class BuyCreditsController : ChameleonControllerBase
    {
        private readonly IRepository<ProxyCreditOrder, Guid> _orderRepository;
        private readonly UserManager _userManager;
        private readonly IProxyCreditGateway _proxyCreditGateway;
        private readonly PayPalClient _payPalClient;

        public BuyCreditsController(
            UserManager userManager,
            IProxyCreditGateway proxyCreditGateway,
            PayPalClient payPalClient,
            IRepository<ProxyCreditOrder, Guid> orderRepository
            )
        {
            _userManager = userManager;
            _proxyCreditGateway = proxyCreditGateway;
            _payPalClient = payPalClient;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index(BuyCreditsOrderDto input)
        {
            var order = await GetOrder(input.Id);
            if (order.ExternalCaptureId != null)
            {
                ViewData["Message"] = "Order was successfully processed.";
            }

            var orderUserId = order.UserId;
            var user = await FindUserByIdAsync(orderUserId.ToString());

            ViewData[nameof(_payPalClient.ClientId)] = _payPalClient.ClientId;
            return View(new BuyCreditsViewModel
            {
                OrderId = input.Id,
                Amount = order.Amount,
                UserEmail = user.EmailAddress
            });
        }

        private async Task<ProxyCreditOrder> GetOrder(Guid orderId)
        {
            var order = await _orderRepository.GetAsync(orderId);
            if (order == null)
            {
                throw new UserFriendlyException($"Order '{orderId}' not found.");
            }
            return order;
        }

        [HttpPost]
        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false, LogError = true)]
        public async Task<CreateOrderResponseDto> CreateOrder(Guid Id)
        {
            var order = await GetOrder(Id);

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(await BuildRequestBody(order));
            //3. Call PayPal to set up a transaction
            var response = await _payPalClient.client().Execute(request);

            var result = response.Result<Order>();
            order.ExternalId = result.Id;
            order.ExternalStatus = result.Status;
            await _orderRepository.UpdateAsync(order);

            return new CreateOrderResponseDto
            {
                Id = result.Id,
                Status = result.Status
            };
        }

        private async Task<OrderRequest> BuildRequestBody(ProxyCreditOrder order)
        {
            var orderUser = await GetOrderUser(order);
            var internalId = order.Id.ToString();
            var amount = order.Amount.ToString("0.00");
            var сurrencyCode = "USD";

            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = "EXAMPLE INC",
                    LandingPage = "BILLING",
                    UserAction = "CONTINUE",
                    ShippingPreference = "NO_SHIPPING"
                },
                Payer = new Payer
                {
                    Email = orderUser.EmailAddress
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId =  internalId,
                        Description = "Buy Proxy Credits",
                        CustomId = orderUser.Id.ToString(),
                        SoftDescriptor = "Buy Proxy Credits",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = сurrencyCode,
                            Value = amount,
                        }
                    }
                },
            };

            return orderRequest;
        }

        private async Task<User> FindUserByIdAsync(string userId)
        {
            User user = null;

            using (UnitOfWorkManager.Current.DisableFilter(
                AbpDataFilters.MustHaveTenant, AbpDataFilters.MayHaveTenant
                ))
            {
                user = await _userManager.FindByIdAsync(userId);
            }

            if (user == null)
            {
                throw new UserFriendlyException($"User with id {userId} not found.");
            }

            return user;
        }

        private async Task<User> GetOrderUser(ProxyCreditOrder order)
        {
            var userId = order.UserId.ToString();

            return await FindUserByIdAsync(userId);
        }

        [HttpPost]
        [DontWrapResult(WrapOnError = false, WrapOnSuccess = false, LogError = true)]
        public async Task<CaptureOrderResponseDto> CaptureOrder(Guid Id)
        {
            var order = await GetOrder(Id);
            var orderUser = await GetOrderUser(order);

            var request = new OrdersCaptureRequest(order.ExternalId);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());
            
            //3. Call PayPal to capture an order
            var response = await _payPalClient.client().Execute(request);

            //4. Save the capture ID to your database. Implement logic to save capture to your database for future reference.
            var result = response.Result<Order>();

            var purchaseCapture = result
                .PurchaseUnits.First()
                .Payments.Captures.First()
                ;

            order.ExternalCaptureId = purchaseCapture.Id;
            order.ExternalInvoiceId = purchaseCapture.InvoiceId;
            order.ExternalStatus = result.Status;
            await _orderRepository.UpdateAsync(order);

            await _proxyCreditGateway.GiveBalanceAsync(orderUser, order.Amount);

            return new CaptureOrderResponseDto
            {
                Id = result.Id,
                Status = result.Status
            };
        }
    }
}

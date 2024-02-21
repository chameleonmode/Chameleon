function getBuyCreditOrderId() {
    return new URLSearchParams(window.location.search).get('id');
}

function ajaxBuyCreditsOrder(url) {
    var entityId = getBuyCreditOrderId();
    return $.ajax({
        url: url + entityId,
        type: 'POST',
        headers: { 'X-Ignore-Headers': '1' }
    });
}

function createBuyCreditsOrder() {
    return ajaxBuyCreditsOrder('/BuyCredits/CreateOrder/');
}

function captureBuyCreditsOrder() {
    return ajaxBuyCreditsOrder('/BuyCredits/CaptureOrder/');
}
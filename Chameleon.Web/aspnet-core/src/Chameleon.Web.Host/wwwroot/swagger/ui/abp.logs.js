var abp = abp || {};
(function () {
    abp.swagger = abp.swagger || {};
    abp.logs = abp.logs || {};

    var requerestType = {
        delete: {
            type: 'DELETE',
            url: '/api/services/app/AppLogger/Delete?Id=',
            errorMessage: 'Failed to delete log!'
        },
        removeAll: {
            type: 'DELETE',
            url: '/api/services/app/AppLogger/RemoveAll',
            errorMessage: 'Failed to delete logs!'
        },
        getAll: {
            type: 'GET',
            url: '/api/services/app/AppLogger/GetAll',
            errorMessage: 'Failed to upload logs!'
        }
    };

    abp.logs.getAll = function (callback) {
        setRequerest(callback, requerestType.getAll);
    }

    abp.logs.delete = function (callback, id) {
        setRequerest(callback, requerestType.delete, id);
    }

    abp.logs.deleteAll = function (callback) {
        setRequerest(callback, requerestType.removeAll);
    }

    function setRequerestHeader(xhr) {
        xhr.setRequestHeader('Abp.TenantId', null);
        xhr.setRequestHeader('Content-type', 'application/json');
    };

    function addAntiForgeryTokenToXhr(xhr) {
        var antiForgeryToken = abp.security.antiForgery.getToken();
        if (antiForgeryToken) {
            xhr.setRequestHeader(abp.security.antiForgery.tokenHeaderName, antiForgeryToken);
        }
    };

    function addAuthTokenToXhr(xhr) {
        var token = abp.auth.getToken();
        if (token) {
            xhr.setRequestHeader(abp.auth.tokenHeaderName, "Bearer " + token);
        }
    };    

    function setRequerest(callback, requerestType, data) {
        var xhr = new XMLHttpRequest();
        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    var responseJSON = JSON.parse(xhr.responseText);
                    var result = responseJSON.result;
                    callback(result);
                } else {
                    alert(requerestType.errorMessage);
                }
            }
        };

        xhr.open(requerestType.type, requerestType.url + (data || ''));
        setRequerestHeader(xhr);
        addAuthTokenToXhr(xhr);
        addAntiForgeryTokenToXhr(xhr);
        xhr.send();
    }
})();

var LoginAction = function () {
    var _Email = $('#Email').val();
    if (_Email == '') {
        FieldValidationAlert('#Email', 'Email Address is Required.', "warning");
        return;
    }
    var _Password = $('#Password').val();
    if (_Password == '') {
        FieldValidationAlert('#Password', 'Password is Required.', "warning");
        return;
    }

    var _frmLogin = $("#frmLogin").serialize();
    $("#btnUserLogin").LoadingOverlay("show", {
        background: "rgba(165, 190, 100, 0.5)"
    });
    $("#btnUserLogin").LoadingOverlay("show");

    $.ajax({
        type: "POST",
        url: "/Account/Login",
        data: _frmLogin,
        success: function (result) {
            if (result.IsSuccess) {
                sessionStorage.setItem("AccessToken", result.AccessToken);
                sessionStorage.setItem("APIBaseURL", result.APIBaseURL);
                sessionStorage.setItem("ClientBaseURL", result.ClientBaseURL);
                SaveUserInfoFromBrowser();
                location.href = "/Dashboard/Index";
            }
            else {
                toastr.error(result.AlertMessage);
                $("#btnUserLogin").LoadingOverlay("hide", true);
            }
        },
        error: function (errormessage) {
            $("#btnUserLogin").LoadingOverlay("hide", true);
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}

var SignOutAction = function () {
    $("#btnSignOut").LoadingOverlay("show", {
        background: "rgba(165, 190, 100, 0.5)"
    });
    $("#btnSignOut").LoadingOverlay("show");

    $.ajax({
        type: "POST",
        url: "/Account/UserSignOut",
        data: null,
        success: function (result) {
            if (result == true) {
                sessionStorage.removeItem("AccessToken");
                location.href = "/Account/Login";
            }
        },
        error: function (errormessage) {
            $("#btnSignOut").LoadingOverlay("hide", true);
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
}


var SaveUserInfoFromBrowser = function () {
    var parser = new UAParser();
    var UserInfoFromBrowser = {};
    var _GetBrowserUniqueID = GetBrowserUniqueID();

    UserInfoFromBrowser.BrowserUniqueID = _GetBrowserUniqueID;
    UserInfoFromBrowser.TimeZone = new Date();
    UserInfoFromBrowser.BrowserMajor = parser.getResult().browser.major;
    UserInfoFromBrowser.BrowserName = parser.getResult().browser.name;
    UserInfoFromBrowser.BrowserVersion = parser.getResult().browser.version;

    UserInfoFromBrowser.CPUArchitecture = parser.getResult().cpu.architecture;
    UserInfoFromBrowser.DeviceModel = parser.getResult().device.model;
    UserInfoFromBrowser.DeviceType = parser.getResult().device.type;
    UserInfoFromBrowser.DeviceVendor = parser.getResult().device.vendor;

    UserInfoFromBrowser.EngineName = parser.getResult().engine.name;
    UserInfoFromBrowser.EngineVersion = parser.getResult().engine.version;
    UserInfoFromBrowser.OSName = parser.getResult().os.name;
    UserInfoFromBrowser.OSVersion = parser.getResult().os.version;
    UserInfoFromBrowser.UA = parser.getResult().ua;

    var _Email = $('#Email').val();
    UserInfoFromBrowser.CreatedBy = _Email;
    UserInfoFromBrowser.ModifiedBy = _Email;

    var _BaseURL = sessionStorage.getItem("APIBaseURL");

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: _BaseURL + "UserInfoFromBrowserAPI/SaveUserInfoFromBrowser",
        data: JSON.stringify(UserInfoFromBrowser),
        dataType: "json",
        success: function (result) {
        },
        error: function (errormessage) {
            SwalSimpleAlert(errormessage.responseText, "warning");
        }
    });
};

var GetBrowserUniqueID = function () {
    var navigator_info = window.navigator;
    var screen_info = window.screen;
    var uid = navigator_info.mimeTypes.length;
    uid += navigator_info.userAgent.replace(/\D+/g, '');
    uid += navigator_info.plugins.length;
    uid += screen_info.height || '';
    uid += screen_info.width || '';
    uid += screen_info.pixelDepth || '';
    return uid;
}
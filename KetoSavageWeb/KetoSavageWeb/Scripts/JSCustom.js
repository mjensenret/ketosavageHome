//debugger;
//var currentUserID;
//function ShowUserProgramDetails(userId) {
//    currentUserID = userId;
//    var url = '@Url.Content("~/UserProgramsController/ShowProgramDetails/' + currentUserID + '")';
//    //userDetailGrid.PerformCallback();
//    window.location.href(url);
//}
function OnDetailGridBeginCallback(s, e) {
    e.customArgs["_userId"] = currentUserID;
}
function OnDetailGridEndCallback(s, e) {
    if (!popup.IsVisible())
        popup.Show();
}
function ShowUserProgramPopup(url) {
    popup.SetContentUrl(url);
    popup.Show();
}
//debugger;
function ShowUserProgramDetails(url) {
    window.location.replace(url);
}

function LeftMenuClick(s, e) {
    if (e.item.name === "Dashboard")
    {
        var url = '@Url.Action("Index", "UserProgress")';   
        window.location.replace(url);
    }
    else
    {
        var pageUrl = $("#enterMacros").val();
        openEntryWindow(e.item.name, pageUrl);
    }

}

    function redirectToMacros() {
        var url = '@Url.Action("Index", "UserProgress")';
        window.location.replace(url);
    }

    function openEntryWindow(action, pageUrl) {
        var url = pageUrl;
        $.ajax({
            type: "POST",
            url: url,
            data: { buttonName: action },
        traditional: true,
        success: function (response) {
            mainPopupControl.SetContentHtml(response);
            MVCxClientUtils.FinalizeCallback();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('oops, something bad happened');
        }
    });
    window['mainPopupControl'].Show();
}
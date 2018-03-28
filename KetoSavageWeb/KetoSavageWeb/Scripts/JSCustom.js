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

function ShowUserProgramDetails(url) {
    window.location.href(url);
}

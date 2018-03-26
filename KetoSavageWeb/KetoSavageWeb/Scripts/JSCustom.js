debugger;
var currentUserID;
function OnHyperLinkClick(userId) {
    currentUserID = userId;
    //userDetailGrid.PerformCallback();
    userProgramHeader.PerformCallback();
}
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
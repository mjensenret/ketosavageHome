var currentUserID;
function OnHyperLinkClick(userId) {
    currentUserID = userId;
    userDetailGrid.PerformCallback();
}
function OnDetailGridBeginCallback(s, e) {
    e.customArgs["_userId"] = currentUserID;
}
function OnDetailGridEndCallback(s, e) {
    if (!UserProgramDetails.IsVisible())
        UserProgramDetails.Show();
}
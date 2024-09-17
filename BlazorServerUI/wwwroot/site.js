// site.js
window.updateUnreadMessageCount = function (count) {
    const badge = document.querySelector('.unread-message-count');
    if (badge) {
        badge.textContent = count > 0 ? count : '';
    }
};

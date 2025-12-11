mergeInto(LibraryManager.library, {
    IsMobile: function() {
        var userAgent = navigator.userAgent || navigator.vendor || window.opera;

        // Detect iPad even if it identifies as a Mac (iOS 13+ issue)
        var isIOS = /iPad|iPhone|iPod/.test(userAgent) || (navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1);
        
        if (isIOS || /Android/i.test(userAgent)) {
            return true;
        }

        return false;
    }
});

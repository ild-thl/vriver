mergeInto(LibraryManager.library, {
    ToggleFullscreenJS: function() {
        var canvas = document.getElementById("unity-canvas");
        if (!document.fullscreenElement) {
            if (canvas.requestFullscreen) {
                canvas.requestFullscreen();
            } else if (canvas.mozRequestFullScreen) {
                canvas.mozRequestFullScreen();
            } else if (canvas.webkitRequestFullscreen) {
                canvas.webkitRequestFullscreen();
            } else if (canvas.msRequestFullscreen) {
                canvas.msRequestFullscreen();
            }
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }
    },
ShowVRButton: function (show) {
    var btn = document.getElementById("entervr");
    if (btn) {
      btn.style.display = show ? "inline-block" : "none";
    }
  },

  ShowRestartButton: function (show) {
    var btn = document.getElementById("returnbutton");
    if (btn) {
      btn.style.display = show ? "inline-block" : "none";
    }
  },

 WebGLReady: function () {
    console.log("WebGLReady() called from C#");
    // hier kann auch ein Callback oder Event gestartet werden
  },

ForceEnterVR: function () {
    // Zugriff auf die globale unityInstance
    if (typeof unityInstance !== 'undefined' &&
        unityInstance.Module &&
        unityInstance.Module.WebXR) {

	console.log("ForceEnterVR");
      unityInstance.Module.WebXR.toggleVR();
    } else {
      console.warn("ForceEnterVR: unityInstance oder WebXR‑Modul noch nicht bereit");
    }
  }
});



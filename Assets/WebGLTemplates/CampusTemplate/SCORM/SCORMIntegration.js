// Wrapper for SCORM integration
let scormInitialized = false;

const SCORMIntegration = {
    initialize: function () {

        if (!scormInitialized) {
            console.log("Calling SCORM...");
            var scorm = pipwerks.SCORM;
            scorm.version = "1.2"; // SCORM version, change if needed
            console.log("Initializing SCORM...");

            if (typeof pipwerks !== "undefined") {
                pipwerks.debug = { isActive: true };
                console.log("SCORM library loaded successfully.");
            } else {
                console.error("SCORM library is not loaded.");
            }
          
            var success = scorm.init();
            if (success) {
                console.log("SCORM initialized successfully.");
            } else {
                console.error("SCORM initialization failed. Error: ", scorm.debug.getCode());
                console.error("Error String: ", scorm.debug.getInfo());
            }   
        }
    },
    sendScore: function (score) {
        var scorm = pipwerks.SCORM;
        if (scorm.set("cmi.core.score.raw", score)) {
            console.log("Score sent: " + score);
        } else {
            console.error("Failed to send score.");
        }
    },
    completeLesson: function () {
        var scorm = pipwerks.SCORM;
        if (scorm.set("cmi.core.lesson_status", "completed")) {
            console.log("Lesson marked as completed.");
        } else {
            console.error("Failed to set lesson status.");
        }
    },
    terminate: function () {
        var scorm = pipwerks.SCORM;
        if (scorm.quit()) {
            console.log("SCORM session terminated.");
        } else {
            console.error("Failed to terminate SCORM session.");
        }
    }
};

// Attach functions to the global window object for Unity WebGL
window.initializeSCORM = SCORMIntegration.initialize;
window.sendScoreToSCORM = SCORMIntegration.sendScore;
window.setLessonCompleted = SCORMIntegration.completeLesson;
window.terminateSCORM = SCORMIntegration.terminate;

// Ensure SCORM session terminates on browser close or reload
window.addEventListener("beforeunload", function () {
    SCORMIntegration.terminate();
});

mergeInto(LibraryManager.library, {
    
    JS_ScormInitialize: function() {
        return scorm.initialize();
    },

    JS_ScormFinish: function() {
        scorm.finish();
    },

    JS_ScormCommit: function() {
        scorm.commit();
    },

    JS_ScormSetValue: function(element, value) {
        var elementStr = UTF8ToString(element);
        var valueStr = UTF8ToString(value);
        scorm.setValue(elementStr, valueStr);
    }
});
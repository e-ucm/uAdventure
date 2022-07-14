mergeInto(LibraryManager.library, {

  OpenUrl: function (str) {
    window.open(Pointer_stringify(str), "_self");
  },

  ClearUrl: function () {
    // https://stackoverflow.com/questions/824349/how-do-i-modify-the-url-without-reloading-the-page
  },

    GetUrl: function () {
        var returnStr = window.location.href.split('?')[0];
        var bufferSize = lengthBytesUTF8(returnStr) + 1
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    GetCompleteUrl: function () {
        var returnStr = window.location.href;
        var bufferSize = lengthBytesUTF8(returnStr) + 1
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
 
    GetParameter: function(paramId) {
        var urlParams = new URLSearchParams(location.search);
        var param = urlParams.get(Pointer_stringify(paramId));
        console.log("JavaScript read param: " + param);
        if (param == null) {
            return "";
        }
        var bufferSize = lengthBytesUTF8(param) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(param, buffer, bufferSize);
        return buffer;
    }
});
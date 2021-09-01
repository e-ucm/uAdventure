mergeInto(LibraryManager.library, {

  GetProtocol: function () {
    var returnStr = window.location.protocol.slice(0, -1);
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  }
  
});
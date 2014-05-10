(function (W, T) {
    "use strict";
    if (W.addEventListener) {
        W.addEventListener("load", T.wideMode, false);
        W.addEventListener("resize", T.wideMode, false);
    }
    else if (W.attachEvent) {
        W.attachEvent("onload", T.wideMode);
        W.attachEvent("onresize", T.wideMode);
    }
})(this, TJZX);
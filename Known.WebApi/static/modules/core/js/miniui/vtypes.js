/////////////////////////////////////////////////////////////////////
mini.VTypes["plusErrorText"] = "必须大于0";
mini.VTypes["plus"] = function (v) {
    if (v !== null && v !== "")
        return v > 0;
    return true;
};

mini.VTypes["non-negativeErrorText"] = "必须大于等于0";
mini.VTypes["non-negative"] = function (v) {
    if (v !== null && v !== "")
        return v >= 0;
    return true;
};

mini.VTypes["non-zeroErrorText"] = "不能为0";
mini.VTypes["non-zero"] = function (v) {
    if (v !== null && v !== "")
        return v !== 0;
    return true;
};

mini.VTypes["percentErrorText"] = "必须大于等于0，并且要小于100";
mini.VTypes["percent"] = function (v) {
    if (v !== null && v !== "")
        return v >= 0 && v < 100;
    return true;
};
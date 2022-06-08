/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

var isloadOssJs = false;
function OssClient() {
    if (!isloadOssJs) {
        Utils.addJs('/libs/aliyun-oss-sdk.min.js');
        isloadOssJs = true;
    }

    var client = new OSS(OssClient.config);

    this.put = function (path, data, callback) {
        client.put(path, data)
            .then(function (res) {
                callback(res);
            }).catch(function (err) {
                console.log(err);
            });
    }
}

OssClient.config = {
    region: '',
    accessKeyId: '',
    accessKeySecret: '',
    bucket: ''
};
/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.
import "./libs/jquery.js";
import "./libs/ztree/ztree.js";
import "./libs/highcharts.js";
import "./libs/pdfobject.js";
import "./libs/wangEditor.js";

export function showPrompt(message) {
    hello('Known');
    return prompt(message, 'Type anything here');
}

export function hello(name) {
    DotNet.invokeMethodAsync('Known', 'Hello', name).then(data => {
        alert(data);
    });
}

export async function downloadFile(fileName, stream) {
    const arrayBuffer = await stream.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement('a');
    anchor.href = url;
    if (fileName) {
        anchor.download = fileName;
    }
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(url);
}

export async function showPdf(id, stream) {
    const arrayBuffer = await stream.arrayBuffer();
    const blob = new Blob([arrayBuffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    PDFObject.embed(url, '#' + id, { forceIframe: true });
    URL.revokeObjectURL(url);
}

export function showChart(info) {
    //console.log(info);
    Highcharts.chart(info.id, info.option);
}
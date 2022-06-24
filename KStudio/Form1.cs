/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-24     KnownChen
 * ------------------------------------------------------------------------------- */

namespace KStudio;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        webView21.Source = new Uri(Known.Config.App.AppUrl);
    }
}
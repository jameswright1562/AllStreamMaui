using System;
using Microsoft.Maui.Controls;

namespace MoviesApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
#if ANDROID
        Loaded += OnLoaded;
#endif
    }

#if ANDROID
    void OnLoaded(object? sender, EventArgs e)
    {
        var handler = blazorWebView?.Handler;
        if (handler?.PlatformView is global::Android.Webkit.WebView wv)
        {
            var settings = wv.Settings;
            settings.JavaScriptCanOpenWindowsAutomatically = false;
            settings.SetSupportMultipleWindows(false);
            wv.SetWebChromeClient(new SafeWebChromeClient());
        }
    }
#endif
}

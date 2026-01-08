using System;
#if ANDROID
using AllStream.Platforms.Android.WebView;
#endif
using Microsoft.Maui.Controls;

namespace AllStream;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
#if ANDROID
        Loaded += OnLoaded;
        Padding = new Thickness(0, GetStatusBarHeightDp(), 0, 0);

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
            settings.JavaScriptEnabled = true;
            settings.DomStorageEnabled = true;
            settings.DatabaseEnabled = true;
            settings.MediaPlaybackRequiresUserGesture = false;
            settings.MixedContentMode = global::Android.Webkit.MixedContentHandling.AlwaysAllow;
            global::Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
            wv.SetWebChromeClient(new SafeWebChromeClient());
        }
    }

    static double GetStatusBarHeightDp()
    {
        var res = Android.App.Application.Context.Resources;
        var id = res.GetIdentifier("status_bar_height", "dimen", "android");

        var px = id > 0 ? res.GetDimensionPixelSize(id) : 0;
        var density = res.DisplayMetrics.Density;

        return density > 0 ? px / density : 0;
    }
#endif
}

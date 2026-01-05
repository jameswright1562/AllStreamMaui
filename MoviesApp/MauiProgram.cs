using System;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Storage;
using MoviesApp.Services;
using MoviesApp.Shared.Models;
#if ANDROID
using Android.Webkit;
using Microsoft.Maui.ApplicationModel;
#endif

namespace MoviesApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping(
                    "Adblock",
                    (handler, view) =>
                    {
#if ANDROID
                        //var webView = handler.PlatformView; // Android.Webkit.WebView

                        //webView.SetWebViewClient(new SafeWebViewClient());
                        //webView.SetWebChromeClient(new SafeWebChromeClient());
                        //webView.Settings.JavaScriptEnabled = true;
                        //webView.Settings.DomStorageEnabled = true;
                        //webView.Settings.DatabaseEnabled = true;
                        //webView.Settings.MediaPlaybackRequiresUserGesture = false;
                        //webView.Settings.MixedContentMode =
                        //    Android.Webkit.MixedContentHandling.AlwaysAllow;
#endif

#if WINDOWS
                        var wv2 = handler.PlatformView; // WebView2
                        wv2.CoreWebView2Initialized += (f, e) =>
                        {
                            AdBlock.Configure(f.CoreWebView2);
                        };
#endif
                    }
                );
            });
        builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        Settings settingsFromJson = builder.Configuration.Get<Settings>() ?? new Settings();
        builder.Services.AddSingleton(settingsFromJson);
        try
        {
            using var s = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;
            if (s != null)
            {
                builder.Configuration.AddJsonStream(s);
            }
        }
        catch { }
        MoviesApp.Shared.Services.ServiceBuilder.AddSharedServices(
            builder.Services,
            sp => new FormFactor(),
            builder.Configuration.Get<Settings>() ?? settingsFromJson
        );

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

#if ANDROID
internal sealed class SafeWebViewClient : WebViewClient
{
    public override bool ShouldOverrideUrlLoading(
        Android.Webkit.WebView view,
        IWebResourceRequest request)
    {
        var url = request?.Url;
        if (url == null)
            return false;

        var scheme = url.Scheme?.ToLowerInvariant();
        var host = url.Host?.ToLowerInvariant();

        // Allow ALL internal Blazor + WebView traffic
        if (scheme == "file" ||
            scheme == "about" ||
            scheme == "data" ||
            scheme == "blob" ||
            host == "localhost" ||
            host == "appassets.androidplatform.net")
        {
            return false;
        }

        // Only intercept real external links
        if (scheme == "http" || scheme == "https")
        {
            _ = Launcher.Default.OpenAsync(url.ToString());
            return true;
        }

        return false;
    }
}

internal sealed class SafeWebChromeClient : WebChromeClient
{
    public override bool OnCreateWindow(
        Android.Webkit.WebView view,
        bool isDialog,
        bool isUserGesture,
        Android.OS.Message resultMsg)
    {
        // Let WebView decide – Blazor needs this
        return base.OnCreateWindow(view, isDialog, isUserGesture, resultMsg);
    }
}
#endif

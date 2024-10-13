using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using PhoenixAdult.Configuration;
using System.IO;
using MediaBrowser.Model.Drawing;
#if __EMBY__
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
#else
using System.Net.Http;
using Microsoft.Extensions.Logging;

// using Sentry;
#endif

[assembly: CLSCompliant(false)]

namespace PhoenixAdult
{
    internal interface IHasThumbImage
    {
    }

    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages, IHasThumbImage
    {
#if __EMBY__
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, IHttpClient http, ILogManager logger)
#else
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, IHttpClientFactory http, ILogger<Plugin> logger)
#endif
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            Http = http;

#if __EMBY__
            if (logger != null)
            {
                Log = logger.GetLogger(this.Name);
            }
#else
            Log = logger;
            this.ConfigurationChanged += PluginConfiguration.ConfigurationChanged;

            /*
            SentrySdk.Init(new SentryOptions
            {
                Dsn = Consts.SentryDSN,
            });
            */
#endif
        }

#if __EMBY__
        public static IHttpClient Http { get; set; }
#else
        public static IHttpClientFactory Http { get; set; }
#endif

        public static ILogger Log { get; set; }

        public static Plugin Instance { get; private set; }

        public override string Name => "PhoenixAdult";

        public override Guid Id => Guid.Parse("91FDA366-558B-4A18-AAB8-2DC8627D2EFC");

        public Stream GetThumbImage()
        {
            var type = this.GetType();
            return type.Assembly.GetManifestResourceStream(type.Namespace + ".plugin.png");
        }

        public ImageFormat GetThumbImageFormat()
        {
            return ImageFormat.Png;
        }

        public IEnumerable<PluginPageInfo> GetPages() => new[]
        {
            new PluginPageInfo
            {
                Name = "phoenixadult",
                EmbeddedResourcePath = this.GetType().Namespace + ".Configuration.configPage.html",
                DisplayName = "Phoenix Adult",
            },
            new PluginPageInfo
            {
                Name = "phoenixadultjs",
                EmbeddedResourcePath = this.GetType().Namespace + ".Configuration.configPage.js",
            },
        };
    }
}

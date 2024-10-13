using Jellyfin.Plugin.PhoenixAdult.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Drawing;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
// using Sentry;

[assembly: CLSCompliant(false)]

namespace Jellyfin.Plugin.PhoenixAdult
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, IHttpClientFactory http, ILogger<Plugin> logger)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            Http = http;

            Log = logger;
            this.ConfigurationChanged += PluginConfiguration.ConfigurationChanged;

            /*
            SentrySdk.Init(new SentryOptions
            {
                Dsn = Consts.SentryDSN,
            });
            */
        }

        public static IHttpClientFactory Http { get; set; }

        public static ILogger Log { get; set; }

        public static Plugin Instance { get; private set; }

        public override string Name => "PhoenixAdult";

        public override Guid Id => Guid.Parse("dc40637f-6ebd-4a34-b8a1-8799629120cf");

        public IEnumerable<PluginPageInfo> GetPages() => new[]
        {
            new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = $"{this.GetType().Namespace}.Configuration.configPage.html",
            },
        };
    }
}

using Jellyfin.Plugin.PhoenixAdult.Helpers;
using Jellyfin.Plugin.PhoenixAdult.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.PhoenixAdult.Providers
{
    public class MovieImageProvider : IRemoteImageProvider
    {
        public string Name => Plugin.Instance.Name;

        public bool Supports(BaseItem item) => item is Movie;

        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
            => new List<ImageType>
            {
                ImageType.Primary,
                ImageType.Backdrop,
            };

        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            Logger.Debug($"MovieImageProvider-GetImages() Starting ********************");

            IEnumerable<RemoteImageInfo> images = new List<RemoteImageInfo>();

            if (item == null)
            {
                Logger.Debug($"MovieImageProvider-GetImages() Leaving early ********************");
                return images;
            }

            if (!item.ProviderIds.TryGetValue(this.Name, out var externalID))
            {
                Logger.Debug($"MovieImageProvider-GetImages() Leaving early ********************");
                return images;
            }

            var curID = externalID.Split('#');
            if (curID.Length < 3)
            {
                Logger.Debug($"MovieImageProvider-GetImages() Leaving early ********************");
                return images;
            }

            Logger.Debug($"MovieImageProvider-GetImages() externalID: {externalID}");

            var siteNum = new int[2] { int.Parse(curID[0], CultureInfo.InvariantCulture), int.Parse(curID[1], CultureInfo.InvariantCulture) };

            var provider = Helper.GetProviderBySiteID(siteNum[0]);
            if (provider != null)
            {
                try
                {
                    Logger.Debug($"MovieImageProvider-GetImages() Searching for images");
                    images = await provider.GetImages(siteNum, curID.Skip(2).ToArray(), item, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.Error($"MovieImageProvider-GetImages() error: \"{e}\"");

                    await Analytics.Send(
                        new AnalyticsExeption
                        {
                            Request = string.Join("#", curID.Skip(2)),
                            SiteNum = siteNum,
                            Exception = e,
                        }, cancellationToken).ConfigureAwait(false);
                }

                Logger.Debug($"MovieImageProvider-GetImages() Search results: Found {images.Count()} images");

                images = await ImageHelper.GetImagesSizeAndValidate(images, cancellationToken).ConfigureAwait(false);

                Logger.Debug($"MovieImageProvider-GetImages() Clear images returned: {images.Count()} images");
            }

            Logger.Debug($"MovieImageProvider-GetImages() Leaving ********************");

            return images;
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return Helper.GetImageResponse(url, cancellationToken);
        }
    }
}

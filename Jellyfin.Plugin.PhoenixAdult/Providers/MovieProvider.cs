using Jellyfin.Plugin.PhoenixAdult.Helpers;
using Jellyfin.Plugin.PhoenixAdult.Helpers.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.PhoenixAdult.Providers
{
    public class MovieProvider : IRemoteMetadataProvider<Movie, MovieInfo>
    {
        public MovieProvider()
        {
            Database.LoadAll();
        }

        public string Name => Plugin.Instance.Name;

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken)
        {
            Logger.Debug($"MovieProvider-GetSearchResults() Starting ********************");
            Logger.Debug($"MovieProvider-GetSearchResults() searchInfo.Name: {searchInfo.Name}");

            var result = new List<RemoteSearchResult>();

            if (searchInfo == null || string.IsNullOrEmpty(searchInfo.Name))
            {
                Logger.Debug($"MovieProvider-GetSearchResults() Leaving early empty search name ********************");
                return result;
            }

            Logger.Info($"searchInfo.Name: {searchInfo.Name}");

            var title = string.Empty;
            (int[] siteNum, string siteName) site = (null, null);

            if (!string.IsNullOrEmpty(searchInfo.Path) && Plugin.Instance.Configuration.UseFilePath)
            {
                Logger.Info($"searchInfo.Path: {searchInfo.Path}");
                var path = Path.Combine(Path.GetDirectoryName(searchInfo.Path), Path.GetFileNameWithoutExtension(searchInfo.Path));
                foreach (var name in path.Split(Path.DirectorySeparatorChar).Reverse())
                {
                    title = $"{name} {title}";
                    site = Helper.GetSiteFromTitle(name);
                    if (site.siteNum != null)
                    {
                        break;
                    }
                }
            }

            if (site.siteNum == null)
            {
                title = Helper.ReplaceAbbrieviation(searchInfo.Name);
                site = Helper.GetSiteFromTitle(title);
            }

            if (site.siteNum == null)
            {
                string newTitle;
                if (!string.IsNullOrEmpty(Plugin.Instance.Configuration.DefaultSiteName))
                {
                    newTitle = $"{Plugin.Instance.Configuration.DefaultSiteName} {searchInfo.Name}";
                }
                else
                {
                    newTitle = Helper.GetSiteNameFromTitle(searchInfo.Name);
                }

                if (!string.IsNullOrEmpty(newTitle) && !newTitle.Equals(searchInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    Logger.Debug($"MovieProvider-GetSearchResults() newTitle: {newTitle}");

                    title = Helper.ReplaceAbbrieviation(newTitle);
                    site = Helper.GetSiteFromTitle(title);
                }

                if (site.siteNum == null)
                {
                    Logger.Debug($"MovieProvider-GetSearchResults() cannot find site");
                    return result;
                }
            }

            string searchTitle = Helper.GetClearTitle(title, site.siteName),
                   searchDate = string.Empty;
            DateTime? searchDateObj;
            var titleAfterDate = Helper.GetDateFromTitle(searchTitle);

            searchTitle = titleAfterDate.searchTitle;
            searchDateObj = titleAfterDate.searchDateObj;
            if (searchDateObj.HasValue)
            {
                searchDate = searchDateObj.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else
            {
                if (searchInfo.PremiereDate.HasValue)
                {
                    searchDateObj = searchInfo.PremiereDate.Value;
                    searchDate = searchInfo.PremiereDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                }
            }

            if (string.IsNullOrEmpty(searchTitle))
            {
                return result;
            }

            Logger.Debug($"MovieProvider-GetSearchResults() site: {site.siteNum[0]}:{site.siteNum[1]} ({site.siteName})");
            Logger.Debug($"MovieProvider-GetSearchResults() searchTitle: {searchTitle}");
            Logger.Debug($"MovieProvider-GetSearchResults() searchDate: {searchDate}");

            var provider = Helper.GetProviderBySiteID(site.siteNum[0]);
            if (provider != null)
            {
                Logger.Debug($"MovieProvider-GetSearchResults() provider: {provider}");

                try
                {
                    result = await provider.Search(site.siteNum, searchTitle, searchDateObj, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.Error($"MovieProvider-GetSearchResults() Search error: \"{e}\"");

                    await Analytics.Send(
                        new AnalyticsExeption
                        {
                            Request = searchInfo.Name,
                            SiteNum = site.siteNum,
                            SearchTitle = searchTitle,
                            SearchDate = searchDateObj,
                            ProviderName = provider.ToString(),
                            Exception = e,
                        }, cancellationToken).ConfigureAwait(false);
                }

                if (result.Any())
                {
                    Logger.Debug($"MovieProvider-GetSearchResults() Found scenes for searchTitle: {searchTitle}");

                    foreach (var scene in result)
                    {
                        scene.ProviderIds[this.Name] = $"{site.siteNum[0]}#{site.siteNum[1]}#" + scene.ProviderIds[this.Name];
                        scene.Name = scene.Name.Trim();
                        if (scene.PremiereDate.HasValue)
                        {
                            scene.ProductionYear = scene.PremiereDate.Value.Year;
                        }

                        Logger.Debug($"MovieProvider-GetSearchResults() {scene.ProviderIds[this.Name]}: {scene.Name}");
                    }

                    if (result.Any(scene => scene.IndexNumber.HasValue))
                    {
                        result = result.OrderByDescending(o => o.IndexNumber.HasValue).ThenByDescending(o => o.IndexNumber).ToList();
                    }
                    else if (!string.IsNullOrEmpty(searchDate) && result.All(o => o.PremiereDate.HasValue) && result.Any(o => o.PremiereDate.Value != searchDateObj))
                    {
                        result = result.OrderBy(o => Math.Abs((searchDateObj - o.PremiereDate).Value.TotalDays)).ToList();
                    }
                    else
                    {
                        result = result.OrderByDescending(o => 100 - LevenshteinDistance.Calculate(searchTitle, Helper.GetClearTitle(o.Name), StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    Logger.Debug($"MovieProvider-GetSearchResults() Search results: Found {result.Count()} results for searchTitle: {searchTitle}");
                }
                else
                {
                    Logger.Debug($"MovieProvider-GetSearchResults() Search results: No results found for searchTitle: {searchTitle}");
                }
            }

            Logger.Debug($"MovieProvider-GetSearchResults() Leaving  ********************");

            return result;
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken)
        {
            Logger.Debug($"MovieProvider-GetMetadata() Starting ********************");
            Logger.Debug($"MovieProvider-GetMetadata() info.Name: {info.Name}");

            var result = new MetadataResult<Movie>
            {
                HasMetadata = false,
                Item = new Movie(),
            };

            if (info == null)
            {
                Logger.Debug($"MovieProvider-GetMetadata() Leaving early ********************");
                return result;
            }

            DateTime? premiereDateObj = null;
            if (info.PremiereDate.HasValue)
            {
                premiereDateObj = info.PremiereDate.Value;
            }

            string[] curID = null;
            var sceneID = info.ProviderIds;
            if (sceneID.TryGetValue(this.Name, out var externalID))
            {
                curID = externalID.Split('#');
            }

            if ((!sceneID.ContainsKey(this.Name) || curID == null || curID.Length < 3) && !Plugin.Instance.Configuration.DisableAutoIdentify)
            {
                Logger.Debug($"MovieProvider-GetMetadata() Searching for scene");

                var searchResults = await this.GetSearchResults(info, cancellationToken).ConfigureAwait(false);
                if (searchResults.Any())
                {
                    var first = searchResults.First();

                    sceneID = first.ProviderIds;

                    sceneID.TryGetValue(this.Name, out externalID);
                    curID = externalID.Split('#');

                    if (first.PremiereDate.HasValue)
                    {
                        premiereDateObj = first.PremiereDate.Value;
                    }
                }
            }

            if (curID == null)
            {
                Logger.Debug($"MovieProvider-GetMetadata() Cannot find scene");
                return result;
            }
            else
            {
                Logger.Debug($"MovieProvider-GetMetadata() PhoenixAdultID: {curID[0]}:{curID[1]}:{curID[2]}");
            }

            var siteNum = new int[2] { int.Parse(curID[0], CultureInfo.InvariantCulture), int.Parse(curID[1], CultureInfo.InvariantCulture) };

            var provider = Helper.GetProviderBySiteID(siteNum[0]);
            if (provider != null)
            {
                Logger.Debug($"PhoenixAdult ID: {externalID}");

                MetadataResult<BaseItem> res = null;
                try
                {
                    res = await provider.Update(siteNum, curID.Skip(2).ToArray(), cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Logger.Error($"MovieProvider-GetMetadata() Update error: \"{e}\"");

                    await Analytics.Send(
                        new AnalyticsExeption
                        {
                            Request = string.Join("#", curID.Skip(2)),
                            SiteNum = siteNum,
                            SearchTitle = info.Name,
                            SearchDate = premiereDateObj,
                            ProviderName = provider.ToString(),
                            Exception = e,
                        }, cancellationToken).ConfigureAwait(false);
                }

                if (res != null)
                {
                    result.HasMetadata = true;
                    result.Item = (Movie)res.Item;
                    result.People = res.People;
                }

                if (result.HasMetadata)
                {
                    Logger.Debug($"MovieProvider-GetMetadata() Metadata found.");
                    Logger.Debug($"MovieProvider-GetMetadata() Adding provider ID: {this.Name}:{sceneID[this.Name]}");

                    result.Item.OfficialRating = "XXX";
                    result.Item.ProviderIds.Update(this.Name, sceneID[this.Name]);

                    result.Item.Name = HttpUtility.HtmlDecode(result.Item.Name).Trim();

                    if (!string.IsNullOrEmpty(result.Item.Overview))
                    {
                        result.Item.Overview = HttpUtility.HtmlDecode(result.Item.Overview).Trim();
                    }

                    result.Item.AddStudio(Helper.GetSearchSiteName(siteNum));
                    var newStudios = new List<string>();
                    foreach (var studio in result.Item.Studios)
                    {
                        var studioName = studio.Trim();

                        if (studioName.All(char.IsLower))
                        {
                            studioName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(studioName);
                        }

                        if (!newStudios.Contains(studioName, StringComparer.OrdinalIgnoreCase))
                        {
                            newStudios.Add(studioName);
                        }
                    }

                    if (newStudios.Count() > 0)
                    {
                        Logger.Debug($"MovieProvider-GetMetadata() Found {newStudios.Count()} studios");
                    }

                    result.Item.Studios = newStudios.ToArray();

                    if (!result.Item.PremiereDate.HasValue)
                    {
                        result.Item.PremiereDate = premiereDateObj;
                    }

                    if (result.Item.PremiereDate.HasValue)
                    {
                        result.Item.ProductionYear = result.Item.PremiereDate.Value.Year;
                    }

                    if (result.People != null && result.People.Any())
                    {
                        result.People = Actors.Cleanup(result);
                        Logger.Debug($"MovieProvider-GetMetadata() Found {result.People.Count()} actors");
                    }

                    if (result.Item.Genres != null && result.Item.Genres.Any())
                    {
                        result.Item.Genres = Genres.Cleanup(result.Item.Genres, result.Item.Name, result.People);
                        Logger.Debug($"MovieProvider-GetMetadata() Found {result.Item.Genres.Count()} genres");
                    }

                    if (!string.IsNullOrEmpty(result.Item.ExternalId))
                    {
                        result.Item.ProviderIds.Update(this.Name + "URL", result.Item.ExternalId);
                        Logger.Debug($"MovieProvider-GetMetadata() Found {result.Item.ExternalId} URL");
                    }
                }
                else
                {
                    Logger.Debug($"MovieProvider-GetMetadata() No Metadata found.");
                }
            }

            Logger.Debug($"MovieProvider-GetMetadata() Leaving ********************");

            return result;
        }

        public Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return Helper.GetImageResponse(url, cancellationToken);
        }
    }
}

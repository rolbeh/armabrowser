using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using ArmaBrowser.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArmaBrowser.ViewModel
{
    internal class UpdateAvailableViewModel : ObjectNotify
    {
        internal async Task CheckForUpdates()
        {
            var name = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName();
            var currentVersion = name.Version.ToString();

            string releasesJson = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.github.com/");
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name.Name, currentVersion));
                client.DefaultRequestHeaders.Accept.Clear();

                using (var response = await client.GetAsync("repos/sonabit/armabrowser/releases"))
                {
                    if (response.IsSuccessStatusCode)
                        releasesJson = await response.Content.ReadAsStringAsync();
                }
            }
            if (!string.IsNullOrEmpty(releasesJson))
                try
                {
                    var gitHubReleases =
                        JsonConvert.DeserializeObject<GitHubReleaseInfo[]>(releasesJson, new ExpandoObjectConverter());

                    var gitHubReleaseInfo = gitHubReleases.OrderByDescending(r => r.published_at)
                        .FirstOrDefault(info => info.assets.Any(a => a.browser_download_url.EndsWith(".zip")));
                    if (string.IsNullOrEmpty(gitHubReleaseInfo?.tag_name))
                        return;
                    var lastGithubVersion =
                        gitHubReleaseInfo.tag_name.Substring(
                            gitHubReleaseInfo.tag_name.LastIndexOf("_", StringComparison.OrdinalIgnoreCase) + 1);

                    if (new Version(lastGithubVersion) > new Version(currentVersion))
                    {
                        await DownloadUpdateAsync(gitHubReleaseInfo);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
        }

        private async Task DownloadUpdateAsync(GitHubReleaseInfo gitHubReleaseInfo)
        {
            var uri = gitHubReleaseInfo.assets.FirstOrDefault(a => a.browser_download_url.EndsWith(".zip"))
                ?.browser_download_url;
            if (string.IsNullOrEmpty(uri))
                return;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uri);
                    var name = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName();
                    var currentVersion = name.Version.ToString();
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name.Name, currentVersion));
                    client.DefaultRequestHeaders.Accept.Clear();

                    using (var response = await client.GetAsync(uri))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var tempArchiveDirectory = Path.Combine(Path.GetTempPath(), "armabrowserupdates",
                                gitHubReleaseInfo.tag_name);
                            Directory.CreateDirectory(tempArchiveDirectory);
                            var tempArchiveFileName =
                                Path.Combine(tempArchiveDirectory, client.BaseAddress.Segments.Last());
                            using (var fileStream =
                                File.Open(tempArchiveFileName, FileMode.OpenOrCreate))
                            {
                                fileStream.SetLength(0);
                                await response.Content.CopyToAsync(fileStream);
                            }
                            using (ZipArchive zipArchive = ZipFile.OpenRead(tempArchiveFileName))
                            {
                                var zipArchiveEntry = zipArchive.Entries.FirstOrDefault(
                                    entry => entry.FullName.EndsWith("ArmaBrowserUpdater.exe",
                                        StringComparison.OrdinalIgnoreCase));
                                if (zipArchiveEntry?.FullName != null)
                                {
                                    // ReSharper disable once AssignNullToNotNullAttribute
                                    using (var entryStream = zipArchiveEntry.Open())
                                    using (var fileStream = File.OpenWrite(Path.Combine(tempArchiveDirectory,
                                        Path.GetFileName(zipArchiveEntry.FullName))))
                                    {
                                        fileStream.SetLength(0);
                                        await entryStream.CopyToAsync(fileStream);
                                    }
                                }
                                else
                                {
                                    string binDir =
                                        Path.GetDirectoryName(Assembly.GetEntryAssembly().GetName().FullName);
                                    if (binDir != null && File.Exists(Path.Combine(binDir, "ArmaBrowserUpdater.exe")))
                                    {
                                        File.Copy(Path.Combine(binDir, "ArmaBrowserUpdater.exe"),
                                            Path.Combine(tempArchiveDirectory, "ArmaBrowserUpdater.exe"), true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}
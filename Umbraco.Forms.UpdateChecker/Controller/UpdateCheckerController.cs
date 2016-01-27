using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v3.RemoteRepositories;
using NuGet.Versioning;
using NullSettings = NuGet.NullSettings;
using PackageSource = NuGet.PackageSource;
using PackageSourceProvider = NuGet.PackageSourceProvider;

namespace Umbraco.Forms.UpdateChecker.Controller
{

    //TODO: Look at WebAPI Cache Output Attributes
    //Not built into WebAPI
    //From a MS MVP - Filip W (http://www.strathweb.com)
    //https://github.com/filipw/AspNetWebApi-OutputCache


    [RoutePrefix("api/UpdateChecker")]
    public class UpdateCheckerController : ApiController
    {
        [HttpGet]
        [Route("Ping")]
        public string Ping()
        {
            return "Pong";
        }

        [HttpGet]
        [Route("CheckUpdate")]
        public async Task<string> CheckForUpdate(string versionNumber)
        {
            //Check to see if we can find a newer/greater version number on Nuget.org
            //Decide if lazy & do as GET with QS or do as proper POST

            var list = new List<PackageSource>();
            list.Add(new PackageSource(NuGetConstants.V3FeedUrl, NuGetConstants.FeedName));

            var fooList = new List<ResourceProvider>();
            fooList.Add(new RemoteV3FindPackagePackageByIdResourceProvider());

            var packageProvider = NuGet.Protocol.Core.Types.Repository.CreateProvider(fooList);

            //May have one or more definied
            foreach (var repo in packageProvider.GetRepositories())
            {
                var source = repo.PackageSource;
                var otherResource = await repo.GetResourceAsync<NuGet.Protocol.Core.Types.FindPackageByIdResource>();
                var items = await otherResource.GetAllVersionsAsync("UmbracoForms", CancellationToken.None);
                
                var resource = await repo.GetResourceAsync<NuGet.Protocol.Core.Types.MetadataResource>();
                var lastestFormsVersion = await resource.GetLatestVersion("UmbracoForms", false, false, CancellationToken.None);
                

                //Parse the current version so we can compare easier
                var currentVersion = NuGetVersion.Parse(versionNumber);

                if (lastestFormsVersion > currentVersion)
                {
                    //There is a newer version available
                }

            }
            
            return "foo";
        }

    }
}
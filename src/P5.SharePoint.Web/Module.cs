using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Platform.Core.Modularity;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Settings;
using P5.SharePoint.Core;
using P5.SharePoint.Data.Repositories;
using P5.SharePoint.Core.Options;
using Microsoft.Extensions.Options;
using P5.SharePoint.Core.Services;
using System;
using P5.SharePoint.Data.Services;
using Microsoft.Identity.Web;
using MediatR;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;
using P5.SharePoint.Data.Schemas;
using GraphQL.Server;
using P5.SharePoint.Data.Models;

namespace P5.SharePoint.Web;

public class Module : IModule, IHasConfiguration
{
    public ManifestModuleInfo ModuleInfo { get; set; }
    public IConfiguration Configuration { get; set; }

    public void Initialize(IServiceCollection serviceCollection)
    {
        // Initialize database
        var connectionString = Configuration.GetConnectionString(ModuleInfo.Id) ??
                               Configuration.GetConnectionString("VirtoCommerce");

        serviceCollection.AddDbContext<SharePointDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString(ModuleInfo.Id) ??
                                     Configuration.GetConnectionString("VirtoCommerce"));
        });

        serviceCollection.Configure<AuthenticationOptions>(Configuration.GetSection(AuthenticationOptions.SectionName));
        serviceCollection.AddSingleton<ICertificateLoader, DefaultCertificateLoader>();
        serviceCollection.AddSingleton<ConfidentialApplicationService>();
        serviceCollection.AddSchemaBuilder<SharePointSchema>();
        var graphQlbuilder = new CustomGraphQLBuilder(serviceCollection);
        graphQlbuilder.AddGraphTypes(typeof(XSharePointAnchor));

        serviceCollection.AddAutoMapper(typeof(XSharePointAnchor));
        serviceCollection.AddMediatR(typeof(XSharePointAnchor));

        serviceCollection.Configure<SharePointOptions>(Configuration.GetSection(SharePointOptions.SectionName));
        serviceCollection.AddTransient<Data.Services.Schemas.V1.SharePointService>();
        serviceCollection.AddTransient<Data.Services.Schemas.V2.SharePointService>();
        serviceCollection.AddTransient(GetSharePointService);



        // Override models
        //AbstractTypeFactory<OriginalModel>.OverrideType<OriginalModel, ExtendedModel>().MapToType<ExtendedEntity>();
        //AbstractTypeFactory<OriginalEntity>.OverrideType<OriginalEntity, ExtendedEntity>();

        // Register services
        //serviceCollection.AddTransient<IMyService, MyService>();
    }

    public void PostInitialize(IApplicationBuilder appBuilder)
    {
        var serviceProvider = appBuilder.ApplicationServices;

        // Register settings
        var settingsRegistrar = serviceProvider.GetRequiredService<ISettingsRegistrar>();
        settingsRegistrar.RegisterSettings(ModuleConstants.Settings.AllSettings, ModuleInfo.Id);

        // Register permissions
        var permissionsRegistrar = serviceProvider.GetRequiredService<IPermissionsRegistrar>();
        permissionsRegistrar.RegisterPermissions(ModuleConstants.Security.Permissions.AllPermissions
            .Select(x => new Permission { ModuleId = ModuleInfo.Id, GroupName = "SharePoint", Name = x })
            .ToArray());

        // Apply migrations
        using var serviceScope = serviceProvider.CreateScope();
        using var dbContext = serviceScope.ServiceProvider.GetRequiredService<SharePointDbContext>();
        dbContext.Database.Migrate();
    }

    public void Uninstall()
    {
        // Nothing to do here
    }
    private ISharePointService GetSharePointService(IServiceProvider provider)
    {
        var options = provider.GetRequiredService<IOptions<SharePointOptions>>().Value;

        return options.SchemeVersion switch
        {
            SchemeVersion.V1 => provider.GetRequiredService<Data.Services.Schemas.V1.SharePointService>(),
            SchemeVersion.V2 => provider.GetRequiredService<Data.Services.Schemas.V2.SharePointService>(),
            _ => throw new ArgumentOutOfRangeException(nameof(options.SchemeVersion))
        };
    }

}

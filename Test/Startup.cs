using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Funq;
using ServiceStack;
using ServiceStack.Configuration;
using Test.ServiceInterface;
using ServiceStack.Redis;
using ServiceStack.Caching;
using ServiceStack.Auth;
using MongoDB.Driver;
using ServiceStack.Authentication.MongoDb;
using ServiceStack.Data;

namespace Test
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });

        }
    }

    public class AppHost : AppHostBase
    {
        public AppHost() : base("Test", typeof(MyServices).Assembly) { }

        // Configure your AppHost with the necessary configuration and dependencies your App needs
        public override void Configure(Container container)
        {
            SetConfig(new HostConfig
            {
                DefaultRedirectPath = "/metadata",
                DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
            });

            container.Register<IRedisClientsManager>(c =>
                new RedisManagerPool("redis:6379"));
            
            container.Register(c => c.Resolve<IRedisClientsManager>().GetCacheClient());
            
            var connectionString = "mongodb://mongo";
            var dbName = "testdb";
            var db = new MongoClient(connectionString)
                .GetDatabase(dbName);
            
            container.Register(db);
            
            container.Register<IUserAuthRepository>(new MongoDbAuthRepository(db, true));
            
            Plugins.Add(new AuthFeature(() => 
                new AuthUserSession(), 
                new IAuthProvider[]
                {
                    new BasicAuthProvider(),
                    new CredentialsAuthProvider(), 
                }
            ));
            
            Plugins.Add(new RegistrationFeature()); 
        }
    }
}

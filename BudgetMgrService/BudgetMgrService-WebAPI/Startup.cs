//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using BudgetMgrServiceLib.Models;
//using BudgetMgrService_WebAPI.Models;
//using BudgetMgrService_WebAPI.Processors;
//using BudgetMgrService_WebAPI.Contracts;

//namespace BudgetMgrService_WebAPI
//{
//    public class Startup
//    {
//        public Startup(IHostingEnvironment env)
//        {
//            var builder = new ConfigurationBuilder()
//                .SetBasePath(env.ContentRootPath)
//                .AddJsonFile("testdatasettings.json", optional: false, reloadOnChange: true)
//                .AddJsonFile("grouptypesettings.json", optional: false, reloadOnChange: true)
//                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
//                .AddEnvironmentVariables();
//            Configuration = builder.Build();
//        }

//        public IConfiguration Configuration { get; }

//        // This method gets called by the runtime. Use this method to add services to the container.
//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddOptions();
//            services.Configure<TestDataCfg>(Configuration.GetSection("TestDataCfg"));
//            services.AddDbContext<TransactionContext>(opt => opt.UseInMemoryDatabase("transactions"));
//            services.AddCors(options =>
//            {
//                options.AddPolicy("AllowSpecificOrigin",
//                                  builder => builder.WithOrigins("http://localhost:4200")
//                                                    .AllowAnyMethod()
//                                                    .AllowAnyHeader());
//            });


//            services.ConfigureLoggerService();
//            services.AddMvc();

//            // Register the Swagger generator, defining one or more Swagger documents
//            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info { Title = "BudgetMgr Web API", Version = "v1" }));
//        }

//        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
//        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, TransactionContext context, ILoggerManager logger)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }

//            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
//            loggerFactory.AddDebug();

//            app.ConfigureExceptionHandler(logger);

//            // Shows UseCors with CorsPolicyBuilder for our locally-hosted webclient...
//            app.UseCors("AllowSpecificOrigin");

//            app.UseMvc();

//            context.SeedWithData(env.ContentRootPath);

//            // Enable middleware to serve generated Swagger as a JSON endpoint.
//            app.UseSwagger();

//            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
//            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetApp Web API"));
//        }
//    }
//}

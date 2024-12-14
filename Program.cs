using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;

using Serilog;
using webapi;

namespace webapi
{

    public class Program
    {
        public static void Main(string[] args)
        {


        //    Log.Logger = new LoggerConfiguration()

//.WriteTo.Console()
 //   .CreateLogger();

   //         try
   //         {
    //            Log.Information("Starting web application");

                var builder = WebApplication.CreateBuilder(args);
                builder.Services.AddOpenTelemetry() // Initialize the SDK   
.WithTracing(builder => builder
.AddAspNetCoreInstrumentation(options =>
{
    options.EnrichWithHttpRequest = (activity, httpRequest) =>
    {
        activity.SetTag("requestProtocol", httpRequest.Protocol);
    };
    options.RecordException = true;
    options.EnrichWithHttpResponse = (activity, httpResponse) =>
    {
        activity.SetTag("responseLength", httpResponse.ContentLength);
    };
})

    .AddHttpClientInstrumentation((options) =>
    {
        options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) => activity.SetTag("requestVersion",
                    httpRequestMessage.Version);
        options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) => activity.SetTag("requestVersion",
                    httpResponseMessage.Version);
        options.EnrichWithException = (activity, exception) => activity.SetTag("stackTrace", exception.StackTrace);
    })
    .AddConsoleExporter()
     .AddOtlpExporter());
                builder.Services.AddOpenTelemetry()
                 .WithMetrics(builder => builder
                 .AddAspNetCoreInstrumentation()
                 .AddRuntimeInstrumentation()
                 .AddProcessInstrumentation()
                 .AddHttpClientInstrumentation()
          
                 .AddOtlpExporter());
                builder.Logging.AddOpenTelemetry(options =>
                {
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;
                    options.ParseStateValues = true;
                    options.AddOtlpExporter();
                    options.AddConsoleExporter();
                });

      //          builder.Services.AddSerilog(); // <-- Add this line

                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
            //    builder.Services.AddSerilog(); // <-- Add this line
                var app = builder.Build();

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
      //      catch (Exception ex)
      //      {
      //          Log.Fatal(ex, "Application terminated unexpectedly");
      //      }
      //      finally
       //     {
         //       Log.CloseAndFlush();

         //   }
       // }
    }
}



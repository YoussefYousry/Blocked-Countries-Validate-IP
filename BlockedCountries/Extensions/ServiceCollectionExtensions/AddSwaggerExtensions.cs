
namespace BlockedCountries.API.Extensions.ServiceCollectionExtensions
{
    public static class AddSwaggerExtensions
    {
        public static IServiceCollection AddAppSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("BlockedCountries", new() { Title = "BlockedCountries APIs", Version = "v1" });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var group = apiDesc.GroupName;
                    if (!string.IsNullOrEmpty(group))
                        return string.Equals(group, docName, StringComparison.OrdinalIgnoreCase);
                    return docName.Equals("BlockedCountries", StringComparison.OrdinalIgnoreCase);
                });

                var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

    }
}

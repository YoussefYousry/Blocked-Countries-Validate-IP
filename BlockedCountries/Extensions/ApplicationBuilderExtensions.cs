
namespace BlockedCountries.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        public static void UseAppMiddlewares(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseMiddleware<SerilogUserEnricherMiddleware>();
        }

        public static void UseAppSwaggerUI(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/BlockedCountries/swagger.json", "BlockedCountries APIs");
            });
        }
    }
}

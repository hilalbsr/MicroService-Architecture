using ESourcing.Order.Consumers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ESourcing.Order.Extensions
{
    //Middleware
    public static class ApplicationBuilderExtensions
    {
        //Uygulama ayağa kalktığında start/stop işlemlerine göre çalıştırma.
        public static EventBusOrderCreateConsumer Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            //constructor üstünden inject etmiyoruz.
            Listener = app.ApplicationServices.GetService<EventBusOrderCreateConsumer>();

            //app ayağa kalktığında çalışması, kapandığında dispose etmesini sağlıcağız. Yani consume etme!
            var life = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);

            return app;
        }

        //Consume etmesi
        //Servisi dinleyecek
        private static void OnStarted()
        {
            Listener.Consume();
        }

        //Connection dispose edilmesi
        private static void OnStopping()
        {
            Listener.Disconnect();
        }
    }
}

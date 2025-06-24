using EcoMonitor.App.Mapper;
using EcoMonitor.DataAccess;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EcoMonitor.UnitTest
{
    public class TestBase : IDisposable
    {
        protected EcoMonitorDbContext _context { get; private set; }
        protected IMapper _mapper;
        protected IServiceProvider _serviceProvider; //контейнер зависимостей

        public TestBase()
        {
            var services = new ServiceCollection(); //Список сервисов

            /// <summary>
            /// для каждого теста своя БД в памяти
            /// </summary>

            services.AddDbContext<EcoMonitorDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            var config = new TypeAdapterConfig();
            config.Scan(typeof(MappingConfig).Assembly);
            services.AddSingleton(config);
            services.AddSingleton<IMapper, ServiceMapper>();

            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<EcoMonitorDbContext>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();

            SeeData();
        }

        /// <summary>
        /// seedata позволяет заполнять начальными данными 
        /// </summary>
        protected virtual void SeeData() { }

        public void Dispose()
        {
            _context?.Dispose();
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}

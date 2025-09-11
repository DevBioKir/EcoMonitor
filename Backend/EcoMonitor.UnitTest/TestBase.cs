using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Factory.Users;
using EcoMonitor.App.Mapper;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.DataAccess;
using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Pipeline;
using EcoMonitor.Infrastracture.Services;
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
        protected IPasswordHasher _passwordHasher;
        protected IUserFactory _userFactory;
        protected User _user;
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

            // var config = new TypeAdapterConfig();
            // config.Scan(typeof(MappingConfig).Assembly);
            // services.AddSingleton(config);
            // services.AddSingleton<IMapper, ServiceMapper>();

            services.AddSingleton<TypeAdapterConfig>(sp =>
            {
                var config = new TypeAdapterConfig();
                var userFactory = sp.GetRequiredService<IUserFactory>();
                new MappingConfig(userFactory).Register(config);
                return config;
            });
            services.AddScoped<IMapper>(sp =>
            {
                var config = sp.GetRequiredService<TypeAdapterConfig>();
                return new ServiceMapper(sp, config);
            });

            var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwrootTest");
            services.AddSingleton<IImageStorageService>(new ImageStorageService(webRootPath));
            services.AddSingleton<IGeolocationService, GeolocationService>();
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IUserFactory, UserFactory>();

            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<EcoMonitorDbContext>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _passwordHasher = _serviceProvider.GetRequiredService<IPasswordHasher>();
            _userFactory = _serviceProvider.GetRequiredService<IUserFactory>();



            _user = _userFactory.Create(
                "Peter",
                "Petrov",
                "petrov@mail.ru",
                "wadsaf341232sad");

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

using EcoMonitor.App.Abstractions;
using EcoMonitor.App.Factory.Users;
using EcoMonitor.App.Mapper;
using EcoMonitor.Core.Models.Users;
using EcoMonitor.Core.ValueObjects;
using EcoMonitor.DataAccess;
using EcoMonitor.DataAccess.Entities.Users;
using EcoMonitor.DataAccess.Repositories.Users;
using EcoMonitor.Infrastracture.Abstractions;
using EcoMonitor.Infrastracture.Services;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoMonitor.UnitTest
{
    public class TestBase : IDisposable
    {
        protected EcoMonitorDbContext _context { get; private set; }
        protected IMapper _mapper;
        protected IPasswordHasher _passwordHasher;
        protected IUserFactory _userFactory;
        protected User _user;
        protected IUserRepository _userRepository;
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
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddLogging();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<EcoMonitorDbContext>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _passwordHasher = _serviceProvider.GetRequiredService<IPasswordHasher>();
            _userFactory = _serviceProvider.GetRequiredService<IUserFactory>();
            _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            var email = Email.Create("ivanov@mail.ru");

            // _user = _userFactory.Create(
            //     "Peter",
            //     "Petrov",
            //     email.Value,
            //     "somepassword"
            // );
            CreateUser().GetAwaiter().GetResult();
            //SeeData();
        }

        /// <summary>
        /// seedata позволяет заполнять начальными данными 
        /// </summary>
        protected virtual void SeeData() {}

        private async Task CreateUser()
        {
            var email = Email.Create("ivanov@mail.ru");

            _user = _userFactory.Create(
                "Peter",
                "Petrov",
                email.Value,
                "somepassword"
            );

            // Создаем роль с разрешениями
            var roleEntity = new UserRoleEntity
            {
                Id = UserRole.User.Id,
                Name = UserRole.User.Name,
                Description = UserRole.User.Description,
                Permissions = UserRole.User.Permissions
                    .Select(p => new PermissionEntity { Code = p.Code })
                    .ToList()
            };

            await _context.UserRoles.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            // Мапим пользователя в сущность и связываем с ролью
            var userEntity = _mapper.Map<UserEntity>(_user);
            userEntity.RoleId = roleEntity.Id;
            userEntity.Role = roleEntity;

            // Добавляем пользователя в базу и сохраняем
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
            if (_serviceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}

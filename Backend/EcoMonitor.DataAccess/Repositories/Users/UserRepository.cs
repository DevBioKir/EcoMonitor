using MapsterMapper;

namespace EcoMonitor.DataAccess.Repositories.Users
{
    public class UserRepository
    {
        private readonly EcoMonitorDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(
            EcoMonitorDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

    }
}

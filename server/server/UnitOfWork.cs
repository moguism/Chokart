namespace server.Repositories;

public class UnitOfWork
{
    private readonly Context _context;
    private UserRepository _userRepository;
    private StateRepository _stateRepository;
    private FriendshipRepository _friendshipRepository;
    private BattleRepository _battleRepository;
    private UserBattleRepository _userBattleRepository;
    private BattleStateRepository _battleStateRepository;
    private BattleResultRepository _battleResultRepository;
    private CharacterRepository _characterRepository;
    private TrackRepository _trackRepository;

    // poner todos los repositorys

    public UserRepository UserRepository => _userRepository ??= new UserRepository(_context);
    public StateRepository StateRepositoty => _stateRepository ??= new StateRepository(_context);
    public FriendshipRepository FriendshipRepository => _friendshipRepository ??= new FriendshipRepository(_context);
    public BattleRepository BattleRepository => _battleRepository ??= new BattleRepository(_context);
    public UserBattleRepository UserBattleRepository => _userBattleRepository ??= new UserBattleRepository(_context);
    public BattleStateRepository BattleStateRepository => _battleStateRepository ??= new BattleStateRepository(_context);
    public BattleResultRepository BattleResultRepository => _battleResultRepository ??= new BattleResultRepository(_context);
    public CharacterRepository CharacterRepository => _characterRepository ??= new CharacterRepository(_context);
    public TrackRepository TrackRepository => _trackRepository ??= new TrackRepository(_context);

    // poner todos los repositorys

    public UnitOfWork(Context context)
    {
        _context = context;
    }

    public Context Context => _context;

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}

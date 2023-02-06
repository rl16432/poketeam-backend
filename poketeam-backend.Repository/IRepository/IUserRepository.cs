namespace poketeam_backend.Repository.IRepository
{
    public interface IUserRepository<T> : IRepository<T> where T : class
    {
        T GetByUserName(string userName);
        Task<T> GetByUserNameAsync(string userName);
    }
}
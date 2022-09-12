namespace msa_phase_3_backend.Services.ICustomServices
{
    public interface IUserCustomService<T> : ICustomService<T> where T : class
    {
        T GetByUserName(string userName);

        void DeleteById(int id);
    }
}
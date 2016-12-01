using Dapper;

namespace IA.Bts.Portal.Repository
{
    public abstract class BaseRepository
    {
        static BaseRepository()
        {
            Dapper.SqlMapper.AddTypeMap(typeof(string), System.Data.DbType.AnsiString);
        }

        
    }
}
using System.Data;

namespace ChatBox.Models
{
    public abstract class Repository
    {
        protected IDbConnection connection;
        public Repository(IDbConnection connection)
        {
            this.connection = connection;
        }
    }
}

using System.Linq.Expressions;

namespace OrderManager.Data
{
    public class DataHandler : IDisposable
    {
        DBInitialization _initializedDB;

        public DataHandler(DBInitialization initializedDB)
        {
            this._initializedDB = initializedDB ?? throw new ArgumentNullException(nameof(initializedDB));
        }

        public void Dispose()
        {
            if (_initializedDB == null)
            {
                return;
            }
            _initializedDB.Dispose();
        }

        public IQueryable<TEntity> GetData<TEntity>(Expression<Func<TEntity, bool>> whereExpression = null,
            Expression<Func<TEntity, object>> order = null,
            int startindex = 0, int count = 0)
            where TEntity : class
        {
            var query = _initializedDB.Set<TEntity>().AsQueryable();

            if (whereExpression != null) query = query.Where(whereExpression);
            if (order != null) query = query.OrderBy(order);
            if (startindex > 0) query = query.Skip(startindex);
            if (count > 0) query = query.Take(count);

            return query;
        }

        public int GetTableSize<TEntity>() where TEntity : class
        {
            int _tableSize = _initializedDB.Set<TEntity>().AsQueryable().Count();
            return _tableSize;
        }
    }
}

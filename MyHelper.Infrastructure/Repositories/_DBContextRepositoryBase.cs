using MyHelper.Domain.Entities;
using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.MyHelperDbContext;
using Microsoft.EntityFrameworkCore;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Infrastructure.Repositories
{
    /// <summary>
    /// 標準的なDBコンテキストリポジトリの基本クラス
    /// </summary>
    /// <typeparam name="T">エンティティの型</typeparam>
    public abstract class DBContextRepositoryBase<T> : IDataRepositoryBase<T>, IDisposable
            where T : class, IDbContextEntity
    {
        /// <summary>
        /// DBコンテキスト
        /// </summary>
        protected readonly MyHelperDBContext _context;
        
        private static bool _isFirstTime = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DBContextRepositoryBase()
        {
            _context = new MyHelperDBContext();

            if (_isFirstTime)
            {
                _isFirstTime = false;
                // 初回起動時にマイグレーションを自動的に適用
                using (var tmpcontext = new MyHelperDBContext())
                {
                    // ここでエラーが出たということは、おそらくMyHelper.Domain\Entitiesの中身に何か変更を加えたという事だと思います。
                    // MyHelper.Infrastructure下にある
                    // 「テーブルを追加したり更新したりしたい場合.md」
                    // を参照してマイグレーションを行ってください。
                    tmpcontext.Database.Migrate();
                }
            }
        }

        /// <summary>
        /// TransactionManagerを呼び出す
        /// </summary>
        public ITransactionManager TransactionManager => new EfTransactionManager(_context);

        /// <summary>
        /// 指定された条件に一致するエンティティが存在するかどうかを確認します。
        /// </summary>
        /// <param name="predicate">条件を表す式</param>
        /// <returns>エンティティが存在する場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> Any(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        /// <summary>
        /// すべてのエンティティを非同期で取得します。
        /// </summary>
        /// <returns>エンティティのリスト</returns>
        public virtual async Task<List<T>?> GetAllAsync(Expression<Func<T, object>>? orderby = null)
        {
            var ret = _context.Set<T>().AsQueryable();   
            if (orderby != null)
                ret = ret.OrderBy(orderby).AsQueryable();

            return await ret.ToListAsync();
        }

        /// <summary>
        /// 指定された条件に一致するエンティティのリストを非同期で取得します。
        /// </summary>
        /// <param name="where">条件を表す式</param>
        /// <returns>エンティティのリスト</returns>
        public virtual async Task<List<T>?> GetAsync(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>? orderby = null)
        {
            var ret = _context.Set<T>().AsQueryable();
            if (where != null)
                ret = ret.Where(where).AsQueryable();
            if (orderby != null)
                ret = ret.OrderBy(orderby).AsQueryable();

            return await ret.ToListAsync();
        }

        /// <summary>
        /// 指定されたIDのエンティティを非同期で取得します。
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <returns>エンティティ</returns>
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// 指定されたIDのエンティティを削除します。
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <returns>削除に成功した場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>>? where = null)
        {
            var tables = _context.Set<T>().AsQueryable();
            if(where != null)
                tables = tables.Where(where).AsQueryable();
            _context.Set<T>().RemoveRange(tables);
            await _context.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// 全件削除
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAllAsync()
        {
            var tables = _context.Set<T>().AsQueryable();            
            _context.Set<T>().RemoveRange(tables);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 指定されたIDのエンティティを削除します。
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <returns>削除に成功した場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> DeleteByIdAsync(int id)
        {
            var table = await _context.Set<T>().FindAsync(id);
            if (table == null)
            {
                return false;
            }
            _context.Set<T>().Remove(table);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 単一のテーブルを保存します。
        /// </summary>
        /// <param name="id">エンティティのID</param>
        /// <param name="item">エンティティ</param>
        /// <returns>保存に成功した場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> AddOrUpdateAsync(int id, T table)
        {
            var existingTable = await _context.Set<T>().FindAsync(id);
            if (existingTable == null)
            {
                //無ければ追加
                await _context.Set<T>().AddAsync(table);
            }
            else
            {
                //あれば更新
                table.Id = id;
                _context.Entry(existingTable).CurrentValues.SetValues(table);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// すべてのエンティティを更新します。
        /// </summary>
        /// <param name="tables">エンティティのリスト</param>
        /// <returns>更新に成功した場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> UpdateAllItemsAsync(IReadOnlyList<T> tables)
        {

            _context.Set<T>().UpdateRange(tables);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 複数のエンティティを一括で挿入します。
        /// </summary>
        /// <param name="tables">挿入するエンティティのリスト</param>
        /// <returns>挿入に成功した場合はtrue、それ以外の場合はfalse</returns>
        public virtual async Task<bool> BulkInsertAsync(IList<T> tables)
        {         
            await _context.BulkInsertAsync(tables);
            return true;           
        }

        /// <summary>
        /// リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}

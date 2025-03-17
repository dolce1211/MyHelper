using MyHelper.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Repositories
{

    /// <summary>
    /// データリポジトリの基本インターフェース。
    /// </summary>
    /// <typeparam name="T">エンティティの型。</typeparam>
    public interface IDataRepositoryBase<T>:IDisposable
        where T : class, IDbContextEntity
    {
        /// <summary>
        /// トランザクションマネージャを取得
        /// </summary>
        ITransactionManager TransactionManager { get; }

        /// <summary>
        /// 条件に一致するエンティティのリストを取得
        /// </summary>
        /// <param name="where">フィルタ条件。</param>
        /// <param name="orderby">並び替え条件。</param>
        /// <returns>エンティティのリスト。</returns>
        Task<List<T>?> GetAsync(Expression<Func<T, bool>>? where = null, Expression<Func<T, object>>? orderby = null);

        /// <summary>
        /// 指定された条件に一致するエンティティが存在するかどうかを確認
        /// </summary>
        /// <param name="predicate">条件。</param>
        /// <returns>エンティティが存在する場合はtrue、それ以外の場合はfalse。</returns>
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// すべてのエンティティのリストを取得
        /// </summary>
        /// <param name="orderby">並び替え条件。</param>
        /// <returns>エンティティのリスト。</returns>
        Task<List<T>?> GetAllAsync(Expression<Func<T, object>>? orderby = null);

        /// <summary>
        /// 指定されたIDのエンティティを取得
        /// </summary>
        /// <param name="id">エンティティのID。</param>
        /// <returns>エンティティ。</returns>
        Task<T?> GetByIdAsync(int id);


        /// <summary>
        /// 指定されたエンティティを削除
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Expression<Func<T, bool>>? where = null);
        /// <summary>
        /// 指定されたIDのエンティティを削除
        /// </summary>
        /// <param name="id">エンティティのID。</param>
        /// <returns>削除が成功した場合はtrue、それ以外の場合はfalse。</returns>
        Task<bool> DeleteByIdAsync(int id);
 
        /// <summary>
        /// 全件削除
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteAllAsync();
        /// <summary>
        /// 指定されたエンティティを保存
        /// </summary>
        /// <param name="id">エンティティのID。</param>
        /// <param name="setting">エンティティ。</param>
        /// <returns>保存が成功した場合はtrue、それ以外の場合はfalse。</returns>
        Task<bool> AddOrUpdateAsync(int id, T setting);

        /// <summary>
        /// すべてのエンティティを更新
        /// </summary>
        /// <param name="tables">エンティティのリスト。</param>
        /// <returns>更新が成功した場合はtrue、それ以外の場合はfalse。</returns>
        Task<bool> UpdateAllItemsAsync(IReadOnlyList<T> tables);

        /// <summary>
        /// 複数のエンティティを一括で挿入します。
        /// </summary>
        /// <param name="tables">挿入するエンティティのリスト</param>
        /// <returns>挿入に成功した場合はtrue、それ以外の場合はfalse</returns>
        Task<bool> BulkInsertAsync(IList<T> tables);
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Domain.Repositories
{
    /// <summary>
    /// トランザクションを表すインターフェース。
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// 非同期操作としてトランザクションをコミットします。
        /// </summary>
        /// <returns>非同期操作のタスク。</returns>
        Task CommitAsync();

        /// <summary>
        /// 非同期操作としてトランザクションをロールバックします。
        /// </summary>
        /// <returns>非同期操作のタスク。</returns>
        Task RollbackAsync();
    }

    /// <summary>
    /// トランザクションマネージャーを表すインターフェース。
    /// </summary>
    public interface ITransactionManager
    {
        /// <summary>
        /// 非同期操作として新しいトランザクションを開始します。
        /// </summary>
        /// <returns>新しいトランザクションのインスタンス。</returns>
        Task<ITransaction> BeginTransactionAsync();
    }
}

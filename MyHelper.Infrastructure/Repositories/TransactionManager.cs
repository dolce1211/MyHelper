using MyHelper.Domain.Repositories;
using MyHelper.Infrastructure.MyHelperDbContext;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHelper.Infrastructure.Repositories
{
    

    /// <summary>
    /// Entity Frameworkコンテキストのトランザクションを管理します。
    /// </summary>
    public class EfTransactionManager : ITransactionManager
    {
        private readonly MyHelperDBContext _context;

        /// <summary>
        /// <see cref="EfTransactionManager"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context">Entity Frameworkデータベースコンテキスト。</param>
        public EfTransactionManager(MyHelperDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 新しいトランザクションを非同期的に開始します。
        /// </summary>
        /// <returns>非同期操作を表すタスク。タスクの結果はトランザクションを含みます。</returns>
        public async Task<ITransaction> BeginTransactionAsync()
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            return new EfTransaction(transaction);
        }
    }


    /// <summary>
    /// Entity Frameworkコンテキスト内のトランザクションを表します。
    /// </summary>
    public class EfTransaction : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        /// <summary>
        /// <see cref="EfTransaction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="transaction">データベースコンテキストトランザクション。</param>
        public EfTransaction(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        /// <summary>
        /// トランザクションを非同期的にコミットします。
        /// </summary>
        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        /// <summary>
        /// トランザクションを非同期的にロールバックします。
        /// </summary>
        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        /// <summary>
        /// トランザクションを破棄します。
        /// </summary>
        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}

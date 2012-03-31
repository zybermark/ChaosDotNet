using System;
using System.Data;
using System.Data.Common;

namespace ChaosDotNet.Data
{
   public class ChaosDbTransaction : DbTransaction, IDisposable
   {
      private DbConnection _conn;
      private DbTransaction _tran;
      private Func<bool> _doInjectFault;

      public ChaosDbTransaction(DbTransaction transaction, DbConnection connection, Func<bool> doInjectFault)
      {
         if (transaction == null) throw new ArgumentNullException("transaction");
         if (connection == null) throw new ArgumentNullException("connection");
         _tran = transaction;
         _conn = connection;
         _doInjectFault = doInjectFault;
      }

      protected override DbConnection DbConnection
      {
         get { return _conn; }
      }

      internal DbTransaction WrappedTransaction
      {
         get { return _tran; }
      }

      public override IsolationLevel IsolationLevel
      {
         get { return _tran.IsolationLevel; }
      }

      public override void Commit()
      {
         ChaosHelper.DoChaos(_doInjectFault, new Action[] { 
            // make commit fail
            () => { _tran.Rollback(); ChaosDbException.Throw(); } 
         });
         _tran.Commit();
      }

      public override void Rollback()
      {
         _tran.Rollback();
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing && _tran != null)
         {
            _tran.Dispose();
         }
         _tran = null;
         _conn = null;
         base.Dispose(disposing);
      }
   }
}

#pragma warning restore 1591 // xml doc comments warnings
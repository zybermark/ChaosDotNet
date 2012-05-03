using System;
using System.Data;
using System.Data.Common;

namespace ChaosDotNet.Data.Sql
{
   public class SqlChaosDbTransaction : ChaosDbTransaction, IDisposable
   {
      public SqlChaosDbTransaction(DbTransaction transaction, DbConnection connection, Func<bool> doInjectFault):base(transaction,connection,doInjectFault)
      {
      }

      public override void Commit()
      {
         ChaosHelper.DoChaos(_DoInjectFault, new Action[] { 
            // make commit fail
            () => { _Tran.Rollback(); ChaosDbException.Throw(); } 
         });
         _Tran.Commit();
      }

      public override void Rollback()
      {
         _Tran.Rollback();
      }

   }
}

#pragma warning restore 1591 // xml doc comments warnings
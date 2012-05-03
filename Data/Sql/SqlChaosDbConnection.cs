using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace ChaosDotNet.Data.Sql
{
   public class SqlChaosDbConnection : ChaosDbConnection, IDisposable, ICloneable
   {
      public SqlChaosDbConnection(DbConnection connection) : this(connection,  ChaosHelper.DefaultDoInjectFault)
      {}

      public SqlChaosDbConnection(DbConnection connection, Func<bool> doInjectFaultAction):base(connection, doInjectFaultAction)
      {
      }


#pragma warning disable 1591 // xml doc comments warnings



      public override void Close()
      {
         // TODO: Find out if Close throws exceptions?
         //ChaosHelper.DoChaos(_DoInjectFault, new Action[] { ChaosDbException.Throw, () => { throw new ChaosDbException(); } }); 
         _Conn.Close();
      }

      public override void EnlistTransaction(System.Transactions.Transaction transaction)
      {
         _Conn.EnlistTransaction(transaction);
      }

      public override DataTable GetSchema()
      {
         return _Conn.GetSchema();
      }

      public override DataTable GetSchema(string collectionName)
      {
         return _Conn.GetSchema(collectionName);
      }

      public override DataTable GetSchema(string collectionName, string[] restrictionValues)
      {
         return _Conn.GetSchema(collectionName, restrictionValues);
      }

      public override void Open()
      {
         ChaosHelper.DoChaos(_DoInjectFault
            , new Action[] {
               ()=>{throw ChaosSqlExceptions.CreateConnectionFailed();},
               () => {throw ChaosSqlExceptions.CreateShutdownInProgress();},
               () => {throw ChaosSqlExceptions.CreateErrorConnecting();},
               () => {throw ChaosSqlExceptions.CreateErrorAfterConnecting();},
               () => {throw ChaosSqlExceptions.CreateEncryptionNotSupported();},
               () => {throw ChaosSqlExceptions.CreateShutdownInProgress();},
               () => {throw ChaosSqlExceptions.CreateAzureServiceBusy();},
               () => {throw ChaosSqlExceptions.CreateAzureDatabaseNotAvailable();},
            }
            );
         _Conn.Open();
      }

      protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
      {
         return new SqlChaosDbTransaction(_Conn.BeginTransaction(isolationLevel), this, _DoInjectFault);
      }

      protected override DbCommand CreateDbCommand()
      {
         return new SqlChaosDbCommand(_Conn.CreateCommand(), this, _DoInjectFault);
      }


      public new SqlChaosDbConnection Clone()
      {
         ICloneable target = _Conn as ICloneable;
         if (target == null) throw new NotSupportedException("Underlying " + _Conn.GetType().Name + " is not cloneable");
         return new SqlChaosDbConnection((DbConnection)target.Clone());
      }
      object ICloneable.Clone() { return Clone(); }

   }
}

#pragma warning restore 1591 // xml doc comments warnings
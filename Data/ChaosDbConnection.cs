using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace ChaosDotNet.Data
{
   public class ChaosDbConnection : DbConnection, IDisposable, ICloneable
   {
      protected DbConnection _conn;
      Func<bool> _doInjectFault;
      /// <summary>
      /// The wrapped database connection 
      /// </summary>
      public DbConnection WrappedConnection
      {
         get { return _conn; }
      }

      public ChaosDbConnection(DbConnection connection) : this(connection,  ChaosHelper.DefaultDoInjectFault)
      {}

      public ChaosDbConnection(DbConnection connection, Func<bool> doInjectFaultAction)
      {
         Debug.Assert(connection != null);
         if (connection == null) throw new ArgumentNullException("connection");
         Debug.Assert(doInjectFaultAction != null);
         if (doInjectFaultAction == null) throw new ArgumentNullException("doInjectFaultAction");

         _doInjectFault = doInjectFaultAction;
         
         _conn = connection;
         _conn.StateChange += StateChangeHandler;
      }


#pragma warning disable 1591 // xml doc comments warnings


      protected override bool CanRaiseEvents
      {
         get { return true; }
      }

      public override string ConnectionString
      {
         get { return _conn.ConnectionString; }
         set { _conn.ConnectionString = value; }
      }

      public override int ConnectionTimeout
      {
         get { return _conn.ConnectionTimeout; }
      }

      public override string Database
      {
         get { return _conn.Database; }
      }

      public override string DataSource
      {
         get { return _conn.DataSource; }
      }

      public override string ServerVersion
      {
         get { return _conn.ServerVersion; }
      }

      public override ConnectionState State
      {
         get { return _conn.State; }
      }

      public override void ChangeDatabase(string databaseName)
      {
         _conn.ChangeDatabase(databaseName);
      }

      public override void Close()
      {
         ChaosHelper.DoChaos(_doInjectFault, new Action[] { ChaosDbException.Throw, () => { throw new ChaosDbException(); } }); 
         _conn.Close();
      }

      public override void EnlistTransaction(System.Transactions.Transaction transaction)
      {
         _conn.EnlistTransaction(transaction);
      }

      public override DataTable GetSchema()
      {
         return _conn.GetSchema();
      }

      public override DataTable GetSchema(string collectionName)
      {
         return _conn.GetSchema(collectionName);
      }

      public override DataTable GetSchema(string collectionName, string[] restrictionValues)
      {
         return _conn.GetSchema(collectionName, restrictionValues);
      }

      public override void Open()
      {
         ChaosHelper.DoChaos(_doInjectFault, new Action[] {ChaosDbException.Throw, ()=>{throw new ChaosDbException();}});
         _conn.Open();
      }

      protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
      {
         return new ChaosDbTransaction(_conn.BeginTransaction(isolationLevel), this, _doInjectFault);
      }

      protected override DbCommand CreateDbCommand()
      {
         return new ChaosDbCommand(_conn.CreateCommand(), this, _doInjectFault);
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing && _conn != null)
         {
            _conn.StateChange -= StateChangeHandler;
            _conn.Dispose();
         }
         _conn = null;
         base.Dispose(disposing);
      }

      void StateChangeHandler(object sender, StateChangeEventArgs e)
      {
         OnStateChange(e);
      }

      public ChaosDbConnection Clone()
      {
         ICloneable target = _conn as ICloneable;
         if (target == null) throw new NotSupportedException("Underlying " + _conn.GetType().Name + " is not cloneable");
         return new ChaosDbConnection((DbConnection)target.Clone());
      }
      object ICloneable.Clone() { return Clone(); }

   }
}

#pragma warning restore 1591 // xml doc comments warnings
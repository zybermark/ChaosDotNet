using System;
using System.Data;
using System.Data.Common;

namespace ChaosDotNet.Data.Sql
{
   public class SqlChaosDbCommand : ChaosDbCommand, ICloneable, IDisposable
   {
      public SqlChaosDbCommand(DbCommand cmd, DbConnection conn, Func<bool> doInjectFault) : base(cmd, conn, doInjectFault)
      {
      }

      public override string CommandText
      {
         get { return _Cmd.CommandText; }
         set { _Cmd.CommandText = value; }
      }

      public override int CommandTimeout
      {
         get { return _Cmd.CommandTimeout; }
         set { _Cmd.CommandTimeout = value; }
      }

      public override CommandType CommandType
      {
         get { return _Cmd.CommandType; }
         set { _Cmd.CommandType = value; }
      }

      protected override DbConnection DbConnection
      {
         get { return _Conn; }
         set
         {
            _Conn = value;
            _Cmd.Connection = value;
         }
      }

      protected override DbParameterCollection DbParameterCollection
      {
         get { return _Cmd.Parameters; }
      }

      protected override DbTransaction DbTransaction
      {
         get { return _Tran; }
         set
         {
            this._Tran = value;
            _Cmd.Transaction = value;
         }
      }

      public override bool DesignTimeVisible
      {
         get { return _Cmd.DesignTimeVisible; }
         set { _Cmd.DesignTimeVisible = value; }
      }

      public override UpdateRowSource UpdatedRowSource
      {
         get { return _Cmd.UpdatedRowSource; }
         set { _Cmd.UpdatedRowSource = value; }
      }


      protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
      {
         _DoChaos();
         return _Cmd.ExecuteReader(behavior);
      }

      public override int ExecuteNonQuery()
      {
         _DoChaos();
         return _Cmd.ExecuteNonQuery();
      }

      public override object ExecuteScalar()
      {
         _DoChaos();
         return _Cmd.ExecuteScalar();
      }

      protected void _DoChaos()
      {
         ChaosHelper.DoChaos(_DoInjectFault, new Action[] {
            () => {throw ChaosSqlExceptions.CreateTransactionDeadlocked();},
            () => {throw ChaosSqlExceptions.CreateTransportErrorReceiving();},
            () => {throw ChaosSqlExceptions.CreateTransportErrorSending();},
            () => {throw ChaosSqlExceptions.CreateAzureErrorProcessing_40197();},
            () => {throw ChaosSqlExceptions.CreateAzureServiceBusy();},
            () => {throw ChaosSqlExceptions.CreateAzureErrorProcessing_40143();},
         });

      }

      public override void Cancel()
      {
         _Cmd.Cancel();
      }

      public override void Prepare()
      {
         _Cmd.Prepare();
      }

      protected override DbParameter CreateDbParameter()
      {
         return _Cmd.CreateParameter();
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing && _Cmd != null)
         {
            _Cmd.Dispose();
         }
         _Cmd = null;
         base.Dispose(disposing);
      }


      public SqlChaosDbCommand Clone()
      { 
         ICloneable target = _Cmd as ICloneable;
         if (target == null) throw new NotSupportedException("Underlying " + _Cmd.GetType().Name + " is not cloneable");
         return new SqlChaosDbCommand((DbCommand)target.Clone(), _Conn, _DoInjectFault);
      }
      object ICloneable.Clone() { return Clone(); }

   }
}

#pragma warning restore 1591 // xml doc comments warnings
using System;
using System.Data;
using System.Data.Common;

namespace ChaosDotNet.Data
{
   public class ChaosDbCommand : DbCommand, ICloneable, IDisposable
   {
      protected DbCommand _cmd;
      protected DbConnection _conn;
      protected DbTransaction _tran;
      Func<bool> _doInjectFault;

      public ChaosDbCommand(DbCommand cmd, DbConnection conn, Func<bool> doInjectFault)
      {
         if (cmd == null) throw new ArgumentNullException("cmd");

         _cmd = cmd;
         _conn = conn;
         _doInjectFault = doInjectFault;

      }

      public override string CommandText
      {
         get { return _cmd.CommandText; }
         set { _cmd.CommandText = value; }
      }

      public override int CommandTimeout
      {
         get { return _cmd.CommandTimeout; }
         set { _cmd.CommandTimeout = value; }
      }

      public override CommandType CommandType
      {
         get { return _cmd.CommandType; }
         set { _cmd.CommandType = value; }
      }

      protected override DbConnection DbConnection
      {
         get { return _conn; }
         set
         {
            _conn = value;
            _cmd.Connection = value;
         }
      }

      protected override DbParameterCollection DbParameterCollection
      {
         get { return _cmd.Parameters; }
      }

      protected override DbTransaction DbTransaction
      {
         get { return _tran; }
         set
         {
            this._tran = value;
            _cmd.Transaction = value;
         }
      }

      public override bool DesignTimeVisible
      {
         get { return _cmd.DesignTimeVisible; }
         set { _cmd.DesignTimeVisible = value; }
      }

      public override UpdateRowSource UpdatedRowSource
      {
         get { return _cmd.UpdatedRowSource; }
         set { _cmd.UpdatedRowSource = value; }
      }


      protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
      {
         // TODO: Figure out all exceptions
         ChaosHelper.DoChaos(_doInjectFault, new Action[] {ChaosDbException.Throw});
         return _cmd.ExecuteReader(behavior);
      }

      public override int ExecuteNonQuery()
      {
         // TODO: Figure out all exceptions
         ChaosHelper.DoChaos(_doInjectFault, new Action[] {ChaosDbException.Throw});
         return _cmd.ExecuteNonQuery();
      }

      public override object ExecuteScalar()
      {
         // TODO: Figure out all exceptions
         ChaosHelper.DoChaos(_doInjectFault, new Action[] { ChaosDbException.Throw });
         return _cmd.ExecuteScalar();
      }

      public override void Cancel()
      {
         _cmd.Cancel();
      }

      public override void Prepare()
      {
         _cmd.Prepare();
      }

      protected override DbParameter CreateDbParameter()
      {
         return _cmd.CreateParameter();
      }

      protected override void Dispose(bool disposing)
      {
         if (disposing && _cmd != null)
         {
            _cmd.Dispose();
         }
         _cmd = null;
         base.Dispose(disposing);
      }


      public ChaosDbCommand Clone()
      { 
         ICloneable target = _cmd as ICloneable;
         if (target == null) throw new NotSupportedException("Underlying " + _cmd.GetType().Name + " is not cloneable");
         return new ChaosDbCommand((DbCommand)target.Clone(), _conn, _doInjectFault);
      }
      object ICloneable.Clone() { return Clone(); }

   }
}

#pragma warning restore 1591 // xml doc comments warnings
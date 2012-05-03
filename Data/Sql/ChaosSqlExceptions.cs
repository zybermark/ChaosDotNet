using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace ChaosDotNet.Data.Sql
{
   /// <summary>
   /// 
   /// 
   /// </summary>
   public static class ChaosSqlExceptions
   {
      public static SqlException CreateConnectionFailed()
      {
         return _CreateSqlException("A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: TCP Provider, error: 0 - A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.)"
            , 10060
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.ConnectionFailed"
            , ""
            , 0
            );
      }

      public static SqlException CreateErrorConnecting()
      {
         return _CreateSqlException("The client was unable to establish a connection because of an error during connection initialization process before login. "
                                + "Possible causes include the following: the client tried to connect to an unsupported version of SQL Server; the server was too busy "
                                + "to accept new connections; or there was a resource limitation (insufficient memory or maximum allowed connections) on the server. "
                                + "(provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)"
            , 233
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.ErrorConnecting"
            , ""
            , 0
            );
      }

      public static SqlException CreateErrorAfterConnecting()
      {
         return _CreateSqlException("A connection was successfully established with the server, but then an error occurred during the login process. "
                                + "(provider: TCP Provider, error: 0 - The specified network name is no longer available.)"
            , 64
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.ErrorAfterConnecting"
            , ""
            , 0
            );
      }

      public static SqlException CreateEncryptionNotSupported()
      {
         return _CreateSqlException("The instance of SQL Server you attempted to connect to does not support encryption."
            , 20
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.EncryptionNotSupported"
            , ""
            , 0
            );
      }

      public static SqlException CreateTransactionDeadlocked()
      {
         return _CreateSqlException("Transaction (Process ID <chaos>) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction."
            , 1205
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.TransactionDeadlocked"
            , ""
            , 0
            );
      }

      public static SqlException CreateTransportErrorReceiving()
      {
         return _CreateSqlException("A transport-level error has occurred when receiving results from the server"
            , 10053
            , 0 
            , 0 
            , "Server.Chaos"
            , "Server.Chaos.TransportErrorReceiving"
            , ""
            , 0
            );
      }

      public static SqlException CreateTransportErrorSending()
      {
         return _CreateSqlException("A transport-level error has occurred when sending the request to the server. (provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.)"
            , 10054
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.TransportErrorSending"
            , ""
            , 0
            );
      }

      public static SqlException CreateShutdownInProgress()
      {
         return _CreateSqlException("SHUTDOWN is in progress.\r\nLogin failed for user 'xyz'. Only administrators may connect at this time."
            , 6005
            , 0 
            , 10 
            , "Server.Chaos"
            , "Server.Chaos.ShutdownInProgress"
            , ""
            , 0
            );
      }

      //**********************
      // SqlAzure Errors
      //**********************
      public static SqlException CreateAzureErrorProcessing_40197()
      {
         return _CreateSqlException("The service has encountered an error processing your request. Please try again. Error code %d."
            , 40197
            , 0 
            , 0
            , "Server.Chaos"
            , "Server.Chaos.Azure.ErrorProcessingRequest_40197"
            , ""
            , 0
            );
      }

      public static SqlException CreateAzureServiceBusy()
      {
         return _CreateSqlException("The service is currently busy. Retry the request after 10 seconds. Code: %d."
            , 40501
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.Azure.ErrorServiceBusy"
            , ""
            , 0
            );
      }

      public static SqlException CreateAzureDatabaseNotAvailable()
      {
         return _CreateSqlException("Database XXXX on server YYYY is not currently available. Please retry the connection later. If the problem persists, contact customer support, and provide them the session tracing ID of ZZZZZ."
            , 40613
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.Azure.DatabaseNotAvailble"
            , ""
            , 0
            );
      }

      public static SqlException CreateAzureErrorProcessing_40143()
      {
         return _CreateSqlException("The service has encountered an error processing your request. Please try again."
            , 40143
            , 0
            , 0
            , "Server.Chaos"
            , "Server.Chaos.Azure.ErrorProcessing_40143"
            , ""
            , 0
            );
      }

/**************************/

      /// <summary>
      /// 
      /// </summary>
      /// <param name="exceptionMessage"></param>
      /// <param name="infoNumber"></param>
      /// <param name="errorState"></param>
      /// <param name="errorClass">The severity level</param>
      /// <param name="server"></param>
      /// <param name="errorMessage"></param>
      /// <param name="procedure"></param>
      /// <param name="lineNumber"></param>
      /// <returns></returns>
      static SqlException _CreateSqlException(string exceptionMessage
             , int infoNumber
             , byte errorState
             , byte errorClass
             , string server
             , string errorMessage
             , string procedure
             , int lineNumber)
      {
         var error = Instantiators.SqlErrorInstantiator.Create(infoNumber,
                        errorState,
                        errorClass,
                        server,
                        errorMessage,
                        procedure,
                        lineNumber);
         var coll = Instantiators.SqlErrorCollectionInstantiator.Create(error);
         var exc = Instantiators.SqlExceptionInstantiator.Create(exceptionMessage, coll);
         return exc;
      }
   }

   

   // http://www.codeproject.com/Tips/179634/Instantiating-an-SqlException
   // http://pietschsoft.com/post/2012/03/02/Unit-Testing-with-SqlException-in-NET-Only-with-help-from-Reflection.aspx
   namespace Instantiators
   {


      public static partial class SqlErrorInstantiator
      {
         private static readonly System.Reflection.ConstructorInfo constructor;

         static SqlErrorInstantiator()
         {
            constructor =
                typeof(System.Data.SqlClient.SqlError).GetConstructor
                (
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance,
                    null,
                    new System.Type[] 
                    { 
                        typeof(int), 
                        typeof(byte), 
                        typeof(byte), 
                        typeof(string), 
                        typeof(string),
                        typeof(string), 
                        typeof(int) 
                    },
                    null
                );

            return;
         }

         public static System.Data.SqlClient.SqlError Create(
             int InfoNumber,
             byte ErrorState,
             byte ErrorClass,
             string Server,
             string ErrorMessage,
             string Procedure,
             int LineNumber
         )
         {
            return
            (
                (System.Data.SqlClient.SqlError)constructor.Invoke
                (
                    new object[]
                    {
                        InfoNumber,
                        ErrorState,
                        ErrorClass,
                        Server,
                        ErrorMessage,
                        Procedure,
                        LineNumber
                    }
                )
            );
         }
      }

      public static partial class SqlErrorCollectionInstantiator
      {
         private static readonly System.Reflection.ConstructorInfo constructor;
         private static readonly System.Reflection.MethodInfo add;

         static SqlErrorCollectionInstantiator()
         {
            constructor =
                typeof(System.Data.SqlClient.SqlErrorCollection).GetConstructor
                (
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance,
                    null,
                    new System.Type[] { },
                    null
                );

            add = typeof(System.Data.SqlClient.SqlErrorCollection).GetMethod
            (
                "Add",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance
            );

            return;
         }

         public static System.Data.SqlClient.SqlErrorCollection Create(
             params System.Data.SqlClient.SqlError[] SqlErrors
         )
         {
            System.Data.SqlClient.SqlErrorCollection result =
                (System.Data.SqlClient.SqlErrorCollection)constructor.Invoke
                (
                    new System.Type[] { }
                );

            foreach (System.Data.SqlClient.SqlError err in SqlErrors)
            {
               add.Invoke(result, new object[] { err });
            }

            return (result);
         }
      }

      public static partial class SqlExceptionInstantiator
      {
         private static readonly System.Reflection.ConstructorInfo constructor;

         static SqlExceptionInstantiator()
         {
            constructor = typeof(System.Data.SqlClient.SqlException).GetConstructor
            (
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance,
                null,
                new System.Type[] 
                { 
                    typeof(string), 
                    typeof(System.Data.SqlClient.SqlErrorCollection) 
                },
                null
            );

            return;
         }

         public static System.Data.SqlClient.SqlException Create(
             string Message,
             System.Data.SqlClient.SqlErrorCollection ErrorCollection
         )
         {
            return
            (
                (System.Data.SqlClient.SqlException)constructor.Invoke
                (
                    new object[]
                    {
                        Message,
                        ErrorCollection
                    }
                )
            );
         }

      }

   }
}
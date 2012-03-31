using System;
using System.Data.Common;

namespace ChaosDotNet.Data
{
   [Serializable]
   public class ChaosDbException : DbException
   {
      public ChaosDbException(): this ("Monkey causes some chaos! Catch the monkey!") { } // or are they gremlins?
      public ChaosDbException(string message) : base(message) { }
      public ChaosDbException(string message, Exception inner) : base(message, inner) { }
      protected ChaosDbException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context)
         : base(info, context) { }

      public static void Throw() { throw new ChaosDbException(); }
   }
}

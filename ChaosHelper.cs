using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChaosDotNet
{
   public static class ChaosHelper
   {
      static Random _rnd = new Random();
      public static void DoChaos(Func<bool> doInjectFault, IList<Action> faults)
      {
         
         if (faults!=null 
            && faults.Count>0
            && doInjectFault()
            )
         {
            Trace.WriteLine("CHAOSMONKEY STRIKES AGAIN!!!!");
            faults[_rnd.Next(faults.Count)]();
         }
      }

      public static bool DefaultDoInjectFault()
      {
         return _rnd.Next(100) < 25; // 25% chance of fail
         //return _rnd.Next(100) < 5; // 5% chance of fail
      }
   }
}

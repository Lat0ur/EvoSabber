using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using StratSearchSabber.Messaging;
using StratSearchSabber.Search;

namespace StratSearchSabber.Logging
{
   class RunningIndividualLog
   {
      private string _logPath;
      private bool _isInitiated;

      public RunningIndividualLog(string logPath)
      {
         _logPath = logPath;
         _isInitiated = false;
      }

      private static void writeText(Stream fs, string s)
      {
         s += "\n";
         byte[] info = new UTF8Encoding(true).GetBytes(s);
         fs.Write(info, 0, info.Length);
      }

      private void initLog(Individual cur)
      {
         _isInitiated = true;

         // Create a log for individuals
         using (FileStream ow = File.Open(_logPath,
                   FileMode.Create, FileAccess.Write, FileShare.None))
         {
            // The data to maintain for individuals evaluated.
            string[] individualLabels = {
                  "Individual",
                  "Parent"
               };

            var dataLabels = individualLabels
               .Concat(OverallStatistics.Properties);
            for(int i=0; i<cur.StrategyData.Length; i++)
            {
               string prefix = String.Format("S{0}:", i);

               var strategyLabels =
                  StrategyStatistics.Properties
                  .Select(x => prefix+x);
               dataLabels = dataLabels.Concat(strategyLabels);
            }
            dataLabels = dataLabels.Concat(CustomStratWeights.Properties);

            writeText(ow, string.Join(",", dataLabels));
            ow.Close();
         }
      }

    	public void LogIndividual(Individual cur)
    	{
         // Put the header on the log file if this is the first
         // individual in the experiment.
         if (!_isInitiated)
            initLog(cur);

         using (StreamWriter sw = File.AppendText(_logPath))
         {
            string[] individualData = {
                  cur.ID.ToString(),
                  cur.ParentID.ToString(),
               };

            var overallStatistics =
               OverallStatistics.Properties
               .Select(x => cur.OverallData.GetStatByName(x).ToString());
            var data = individualData.Concat(overallStatistics);
            foreach (var stratData in cur.StrategyData)
            {
               var strategyData = StrategyStatistics.Properties
                  .Select(x => stratData.GetStatByName(x).ToString());
               data = data.Concat(strategyData);
            }
            var stratWeights =
               CustomStratWeights.Properties
               .Select(x => cur.GetWeights().GetWeightByName(x).ToString());
            data = data.Concat(stratWeights);

            sw.WriteLine(string.Join(",", data));
            sw.Close();
         }
      }
   }
}

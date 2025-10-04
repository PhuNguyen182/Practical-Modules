#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Android;
using Newtonsoft.Json;
using UnityEngine.Pool;
#endif

namespace FatalAid.ApplicationExitTracker
{
    public class ApplicationExitController
    {
        private const int MaxHistoryCount = 30;

        /// <summary>
        /// This method is only available on Android. Use this at the time the game started to retrieve application exit info history.
        /// </summary>
        /// <param name="maxHistoryCount">Maximum number of records that could be included in the report</param>
        public void ReportExitInfoHistory(int maxHistoryCount = MaxHistoryCount)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string packageName = Application.identifier;
            IApplicationExitInfo[] history =
                ApplicationExitInfoProvider.GetHistoricalProcessExitInfo(packageName, pid: 0, maxHistoryCount);

            using var listPool = ListPool<ExitInfoRecord>.Get(out var exitInfoRecords);
            foreach (var record in history)
            {
                exitInfoRecords.Add(new()
                {
                    description = record.description,
                    describeContents = record.describeContents,
                    definingUid = record.definingUid,
                    importance = record.importance,
                    packageUid = record.packageUid,
                    pid = record.pid,
                    processName = record.processName,
                    processStateSummary = record.processStateSummary,
                    pss = record.pss,
                    realUid = record.realUid,
                    reason = record.reason,
                    rss = record.rss,
                    status = record.status,
                    timestamp = record.timestamp,
                    trace = record.trace,
                    traceAsString = record.traceAsString,
                });
            }
            
            ApplicationExitRecord exitRecord = new()
            {
                exitInfoRecords = exitInfoRecords
            };

            string recordJson = JsonConvert.SerializeObject(exitRecord);
            Debug.Log($"Application exit info: {recordJson}");
#endif
        }
    }
}
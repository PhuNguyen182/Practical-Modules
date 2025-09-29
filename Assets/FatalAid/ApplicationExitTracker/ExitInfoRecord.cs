using System;
using System.Collections.Generic;
using UnityEngine.Android;

namespace FatalAid.ApplicationExitTracker
{
    [Serializable]
    public struct ExitInfoRecord
    {
        public string description;
        public int describeContents;
        public int definingUid;
        public ProcessImportance importance;
        public int packageUid;
        public int pid;
        public string processName;
        public sbyte[] processStateSummary;
        public long pss;
        public int realUid;
        public ExitReason reason;
        public long rss;
        public int status;
        public long timestamp;
        public byte[] trace;
        public string traceAsString;
    }

    [Serializable]
    public struct ExitRecord
    {
        public List<ExitInfoRecord> exitInfoRecords;
    }
}
using System;
using System.Collections.Generic;

namespace FatalAid.Android.ApplicationExitTracker
{
    [Serializable]
    public struct ApplicationExitRecord
    {
        public List<ExitInfoRecord> exitInfoRecords;
    }
}
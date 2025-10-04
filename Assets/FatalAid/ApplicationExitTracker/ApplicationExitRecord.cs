using System;
using System.Collections.Generic;

namespace FatalAid.ApplicationExitTracker
{
    [Serializable]
    public struct ApplicationExitRecord
    {
        public List<ExitInfoRecord> exitInfoRecords;
    }
}
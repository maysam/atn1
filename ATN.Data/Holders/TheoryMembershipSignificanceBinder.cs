﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Data
{
    /// <summary>
    /// Used as a data binder for the analysis interface in initiating the theory membership table
    /// </summary>
    public class TheoryMembershipSignificanceBinder
    {
        public int TheoryId { get; set; }
        public long SourceId { get; set; }
        public bool? RAMarkedContributing { get; set; }
        public bool IsMetaAnalysis { get; set; }
        public TheoryMembershipSignificanceBinder(int TheoryId, long SourceId, bool? RAMarkedContributing, bool IsMetaAnalysis)
        {
            this.TheoryId = TheoryId;
            this.SourceId = SourceId;
            this.RAMarkedContributing = RAMarkedContributing;
            this.IsMetaAnalysis = IsMetaAnalysis;
        }
    }
}
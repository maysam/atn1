using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    /// <summary>
    /// Provides services neccessary for adding and retrieving Subjects, as well as attributing a given Subject to a given Source
    /// </summary>
    public class Subjects : DatabaseInterface
    {
        public Subjects(ATNEntities Entities = null)
            : base(Entities)
        {

        }

        /// <summary>
        /// Gets a Subject by its SubjectId
        /// </summary>
        /// <param name="SubjectId">The SubjectId of the Subject to return</param>
        /// <returns>A Subject corresponding to the passed SubjectId</returns>
        public Subject GetSubjectById(int SubjectId)
        {
            return Context.Subjects.SingleOrDefault(s => s.SubjectId == SubjectId);
        }

        /// <summary>
        /// Gets a Subject by its DataSourceSpecificId
        /// </summary>
        /// <param name="DataSourceSpecificId">The DataSourceSpecificId of the Subject to retrieve</param>
        /// <param name="DataSource">The data source with which the passed DataSourceSpecificId corresponds</param>
        /// <returns>The Subject corresponding to the passed DataSourceSpecificId</returns>
        public Subject GetSubjectByDataSourceId(string DataSourceSpecificId, CrawlerDataSource DataSource)
        {
            return Context.Subjects.SingleOrDefault(s => s.DataSourceId == (int)DataSource && s.DataSourceSpecificId == DataSourceSpecificId);
        }

        /// <summary>
        /// Attributes a given Subject to a given Source
        /// </summary>
        /// <param name="SourceId">The Source to attribute a Subject to</param>
        /// <param name="SubjectId">The Subject to attribute to the passed Source</param>
        public void AddSubjectToSource(long SourceId, int SubjectId)
        {
            Source SourceToAddSubject = Context.Sources.SingleOrDefault(s => s.SourceId == SourceId);
            Subject SubjectToAdd = Context.Subjects.SingleOrDefault(s => s.SubjectId == SubjectId);

            Subject MaybeExistingSubject = SourceToAddSubject.Subjects.SingleOrDefault(s => s.SubjectId == SubjectId);

            if (MaybeExistingSubject == null)
            {
                SourceToAddSubject.Subjects.Add(SubjectToAdd);
                Context.SaveChanges();
            }
        }

        /// <summary>
        /// Retrieves a persistence-model attached Subject from the passed Subject; adding the passed one if it was not yet bound
        /// </summary>
        /// <param name="SubjectToAdd">The Subject to bind or retrieve from the persistence model</param>
        /// <returns>A persistence-model attached Subject</returns>
        public Subject GetOrAddSubject(Subject SubjectToAdd)
        {
            Subject MaybeExistingSubject = Context.Subjects.Where(s => s.DataSourceSpecificId == SubjectToAdd.DataSourceSpecificId && s.DataSourceId == SubjectToAdd.DataSourceId).SingleOrDefault();
            if (MaybeExistingSubject == null)
            {
                Context.Subjects.AddObject(SubjectToAdd);
                Context.SaveChanges();
                MaybeExistingSubject = SubjectToAdd;
            }

            return MaybeExistingSubject;
        }
    }
}

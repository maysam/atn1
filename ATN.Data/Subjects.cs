using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATN.Data
{
    public class Subjects : DatabaseInterface
    {
        public Subjects(ATNEntities Entities = null)
            : base(Entities)
        {

        }

        public Subject GetSubjectById(int SubjectId)
        {
            return Context.Subjects.SingleOrDefault(s => s.SubjectId == SubjectId);
        }

        public Subject GetSubjectByDataSourceId(string DataSourceSpecificId, CrawlerDataSource DataSource)
        {
            return Context.Subjects.SingleOrDefault(s => s.DataSourceId == (int)DataSource && s.DataSourceSpecificId == DataSourceSpecificId);
        }

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

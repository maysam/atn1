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
            Context.Subjects.AddObject(SubjectToAdd);
            Context.SaveChanges();

            return SubjectToAdd;
        }

        public Subject GetOrAddSubject(string SubjectText, string DataSourceSpecificId, CrawlerDataSource DataSource)
        {
            Subject MaybeExistingSubject = Context.Subjects.SingleOrDefault(s => s.DataSourceSpecificId == DataSourceSpecificId && s.DataSourceId == (int)DataSource);

            if (MaybeExistingSubject == null)
            {

                Subject SubjectToCreate = new Subject();
                SubjectToCreate.DataSourceId = (int)DataSource;
                SubjectToCreate.DataSourceSpecificId = DataSourceSpecificId;
                SubjectToCreate.SubjectText = SubjectText;

                Context.Subjects.AddObject(SubjectToCreate);
                Context.SaveChanges();

                return SubjectToCreate;
            }
            else
            {
                return MaybeExistingSubject;
            }
        }
    }
}

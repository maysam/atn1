using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using ATN.Data;
using System.ServiceModel.Security;
using System.Net.Http;
using System.Web;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ATN.Crawler.WebCrawler
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ReferenceRelationship", Namespace = "http://schemas.datacontract.org/2004/07/Libra.Service.API")]
    public enum ReferenceRelationship : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Reference = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Citation = 2,
    }

    /// <summary>
    /// An implementation of the ICrawler interface which is capable of using Microsoft Academic Search as a data source
    /// </summary>
    public class MASCrawler : ICrawler
    {
        private const int RetryDelayMilliseconds = 5000;
        private const int RetryLimit = 5;
        private const int MaxResultSize = 100;
        private const string MASAppId = "88e64815fa1d419299bb110f2696bb28";
        HttpClient client = new HttpClient();
        String uri = "https://westus.api.cognitive.microsoft.com/academic/v1.0/graph/search?mode=json";

        async Task<JObject> callMAG(String jsonString)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(jsonString);
            var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PostAsync(uri, content);
            string responseJsonText = await response.Content.ReadAsStringAsync();
            JObject responseJson = JObject.Parse(responseJsonText);
            return responseJson;
        }

        public MASCrawler()
        {
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", MASAppId);
        }

        public CrawlerDataSource GetDataSource()
        {
            return CrawlerDataSource.MicrosoftAcademicSearch;
        }

        public int GetDataSourceId()
        {
            return (int) CrawlerDataSource.MicrosoftAcademicSearch;
        }

        /// <summary>
        /// Retrieves a list of references or citations for the given PublicationId
        /// </summary>
        /// <param name="PublicationId">The MAS-specific unique identifier corresponding to the Publication to retrieve references for</param>
        /// <param name="Relationship">The type of references to retrieve; citations or references</param>
        /// <returns>A list of MAS-specific unique identifiers referencing or citing the Publication provided</returns>

        private JToken getObject(string id)
        {
            String jsonString = "{ \"path\": \"/paper\", \"paper\": { \"select\": [ \"*\" ], \"type\": \"*\", \"id\": ["+id+"]    }}";
            JObject responseJson = callMAG(jsonString).Result;
            JToken token = responseJson["Results"].First().First()["*"];
            if (token.ToString() == "")
            {
                throw new Exception("Invalid Source");
            }
            return token;
        }

        private string[] GetReferences(string PublicationId, ReferenceRelationship Relationship)
        {
            JToken paper = getObject(PublicationId);
            if(Relationship == ReferenceRelationship.Citation) {
                return paper["CitationIDs"].ToObject<String[]>();
            } else {
                return paper["ReferenceIDs"].ToObject<String[]>();
            }
        }
        public string[] GetCitationsBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting citations for publication {0}", PaperId), "Informational");
            return GetReferences(PaperId, ReferenceRelationship.Citation);
        }

        public string[] GetReferencesBySourceId(string PaperId)
        {
            Trace.WriteLine(string.Format("Getting references for publication {0}", PaperId), "Informational");
            return GetReferences(PaperId, ReferenceRelationship.Reference);
        }

        public CompleteSource GetSourceById(string PublicationId)
        {
            JToken RetrievedPublication = getObject(PublicationId);
             
            CompleteSource cs = new CompleteSource();

            Source CanonicalPaper = new Source();
            CanonicalPaper.Abstract = RetrievedPublication["OriginalTitle"].ToObject<String>(); // abstract missing
            CanonicalPaper.ArticleTitle = RetrievedPublication["OriginalTitle"].ToObject<String>();
            CanonicalPaper.DataSourceId = GetDataSourceId();
            CanonicalPaper.DataSourceSpecificId = PublicationId;
            CanonicalPaper.Year = RetrievedPublication["PublishYear"].ToObject<int>();
            CanonicalPaper.SerializedDataSourceResponse = RetrievedPublication.ToString();
            CanonicalPaper.DOI = RetrievedPublication["DOI"].ToString();
            //if(RetrievedPublication.FullVersionURL.Length > 0)
            {
             //   CanonicalPaper.ExternalURL = RetrievedPublication.FullVersionURL.FirstOrDefault();
            }

            List<ATN.Data.Author> Authors = new List<ATN.Data.Author>(RetrievedPublication["AuthorIDs"].ToObject<List<string>>().Count);
            foreach (string AuthorId in RetrievedPublication["AuthorIDs"].ToObject<List<string>>())
            {
                var Author = getObject(AuthorId);
                var AuthorToAdd = new ATN.Data.Author();
                //AuthorToAdd.FirstName = Author.FirstName;
                //AuthorToAdd.LastName = Author.LastName;
                AuthorToAdd.FullName = Author["Name"].ToString();
                AuthorToAdd.DataSourceSpecificId = AuthorId;
                AuthorToAdd.DataSourceId = GetDataSourceId();
                Authors.Add(AuthorToAdd);
            }

            ATN.Data.Journal Journal = new ATN.Data.Journal();
            if (RetrievedPublication["OriginalVenue"].ToString() != "")
            {
                Journal.JournalId = RetrievedPublication["JournalID"].ToObject<int>();
                if (Journal.JournalId == 0)
                {
                    Journal.JournalId = RetrievedPublication["ConferneceID"].ToObject<int>();
                }
                Journal.JournalName = RetrievedPublication["OriginalVenue"].ToString();
                cs.Journal = Journal;
            }

            List<Subject> Subjects = new List<Subject>();
            foreach (string k in RetrievedPublication["Keywords"].ToObject<List<string>>())
            {
                Subject s = new Subject();
                //s.DataSourceSpecificId = k;
                s.DataSourceId = (int)GetDataSource();
                s.SubjectText = k;
                Subjects.Add(s);
            }
            cs.IsDetached = true;
            cs.Authors = Authors.ToArray();
            cs.Subjects = Subjects.ToArray();
            cs.Source = CanonicalPaper;

            return cs;
        }
    }
}

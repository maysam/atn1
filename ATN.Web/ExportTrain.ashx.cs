using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ATN.Data;
using ATN.Export;

namespace ATN.Web
{
    /// <summary>
    /// Summary description for ExportVisualization
    /// </summary>
    public class ExportTrain : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int TheoryId;
            if (Int32.TryParse(context.Request["TheoryId"], out TheoryId))
            {
                Theories t = new Theories();
                context.Response.ContentType = "text/plain";
                ExtendedSource[] TrainSources = t.GetTrainingSourcesForTheory(TheoryId).ToArray();
                MemoryStream ExportStream = new MemoryStream();
                ARFFExporter.Export(TrainSources, TheoryId, ExportStream, false);
                context.Response.AddHeader("Content-Length", ExportStream.Length.ToString());
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + TheoryId + "-train.arff");
                context.Response.BinaryWrite(ExportStream.ToArray());
                context.Response.End();
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
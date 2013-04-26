using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ATN.Export;

namespace ATN.Web
{
    /// <summary>
    /// Summary description for ExportVisualization
    /// </summary>
    public class ExportVisualization : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            int TheoryId;
            if (Int32.TryParse(context.Request["TheoryId"], out TheoryId))
            {
                GraphBuilder gb = new GraphBuilder();
                context.Response.ContentType = "text/xml";
                Graph ExportGraph = gb.GetGraphForTheory(TheoryId, false, false, false, true, true);
                MemoryStream ExportStream = new MemoryStream();
                XGMMLExporter.Export(ExportGraph.Nodes.ToArray(), ExportGraph.Edges.ToArray(), ExportStream);
                context.Response.AddHeader("Content-Length", ExportStream.Length.ToString());
                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + TheoryId + "Pruned.xml");
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
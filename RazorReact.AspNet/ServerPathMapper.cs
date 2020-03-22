using RazorReact.Core;
using System.Web;

namespace RazorReact.AspNet
{
    public class ServerPathMapper : IServerPathMapper
    {
        public string MapServerPath(string path)
        {
            return HttpContext.Current.Server.MapPath(path);
        }
    }
}
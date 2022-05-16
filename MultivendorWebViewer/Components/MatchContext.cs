using MultivendorWebViewer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    public class MatchContext
    {
        public MatchContext(ApplicationRequestContext requestContext)
        {
            RequestContext = requestContext;
        }



        public ApplicationRequestContext RequestContext { get; private set; }


        public DateTime Now { get { return DateTime.Now; } }

        public object Argument { get; set; }
    }
}

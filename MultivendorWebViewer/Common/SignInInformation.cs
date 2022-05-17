using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Common
{
    public class SignInInformation 
    {
        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>     
        [DataMember]
        public string PassWord { get; set; }

        /// <summary>
        /// Gets or sets user id
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets session id
        /// </summary>
        [DataMember]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets authentication token
        /// </summary>
        [DataMember]
        public string AuthenticationToken { get; set; }

        /// <summary>
        /// Gets or sets region
        /// </summary>
        [DataMember]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets CompanyId
        /// </summary>
        [DataMember]
        public string CompanyId { get; set; }

        [DataMember]
        public string CustomerNumber { get; set; }

        [DataMember]
        public string OrganizationNumber { get; set; }

        /// <summary>Redirect application to url</summary>
        [DataMember]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets permissions
        /// </summary>
        [DataMember]
        public string[] Permissions { get; set; }

        /// <summary>
        /// Gets or sets permission groups
        /// </summary>
        [DataMember]
        public string[] PermissionGroups { get; set; }

        /// <summary>Additional data in a list of key/value pairs</summary>
 
    }
}

using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ODataTools.Reader.Common.Model
{
    public class UserCredentialsConfirmation : Confirmation
    {
        public ICredentials UserCredentials { get; set; }
    }
}

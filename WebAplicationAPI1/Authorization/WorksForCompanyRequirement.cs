using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAplicationAPI1.Authorization
{
    public class WorksForCompanyRequirement: IAuthorizationRequirement
    {
        #region Properties
        public string DomainName{ get;  }
        #endregion
        #region Constructors
        public WorksForCompanyRequirement(string domainName)
        {
            DomainName = domainName;
        }


        #endregion

    }
}

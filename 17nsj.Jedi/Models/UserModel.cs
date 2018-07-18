using _17nsj.Jedi.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSysAdmin { get; set; }
        public bool IsAvailable { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public string Affiliation { get; set; }
        public string AffiliationName
        {
            get
            {
                return TeamDomain.GetName(Affiliation);
            }
        }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public virtual ICollection<Tasks> Tasks { get; set; }
    }

}

using Humanizer;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.Models;

namespace ToDoList.Models
{
    public class Tasks
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Display(Name = "Priority")]
        public PriorityEnum Priority { get; set; }
        public enum PriorityEnum
        {
            High,
            Medium,
            Low
        }
        [Display(Name = "Category")]
        public CategoryEnum Category { get; set; }
        public enum CategoryEnum
        {
            Work,
            Personal,
            Home,
            Finance,
            Health,
            Shopping,
            Travel,
            Other
        }
        public bool IsCompleted { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
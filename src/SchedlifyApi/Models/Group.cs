namespace SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Group
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DepartmentId { get; set; }

    // [Required]
    // public int AdministratorId { get; set; }

    [Required]
    public string Name { get; set; }

    // Navigation properties
    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    // [ForeignKey("AdministratorId")]
    // public User Administrator { get; set; }
    public ICollection<Assignment> Assignments { get; set; }
    public ICollection<Class> Classes { get; set; }
}

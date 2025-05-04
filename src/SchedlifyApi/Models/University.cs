namespace SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class University
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(64)]
    public string Name { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Department> Departments { get; set; }
}
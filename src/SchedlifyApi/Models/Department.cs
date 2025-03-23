namespace SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Department
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UniversityId { get; set; }

    [Required]
    [StringLength(64)]
    public string Name { get; set; }

    // Navigation properties
    [ForeignKey("UniversityId")]
    public University University { get; set; }
    public ICollection<Group> Groups { get; set; }
    public ICollection<TemplateSlot> TemplateSlots { get; set; }
}
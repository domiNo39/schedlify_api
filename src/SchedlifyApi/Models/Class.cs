namespace SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Class
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GroupId { get; set; }

    [Required]
    public string Name { get; set; }

    // Navigation properties
    [ForeignKey("GroupId")]
    public Group Group { get; set; }
    public ICollection<Assignment> Assignments { get; set; }
}
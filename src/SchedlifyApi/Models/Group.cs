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

    [Required]
    public string Name { get; set; }

    // Навігаційні властивості
    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    // Колекція для користувачів, які належать до цієї групи
    public ICollection<TgUser> TgUsers { get; set; }

    // Інші колекції
    public ICollection<Assignment> Assignments { get; set; }
    public ICollection<Class> Classes { get; set; }
}

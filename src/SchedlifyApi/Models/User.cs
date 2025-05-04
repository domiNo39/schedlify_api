namespace SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(32)]
    public string Login { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    // Navigation properties
    public ICollection<Group> AdministratedGroups { get; set; }
}
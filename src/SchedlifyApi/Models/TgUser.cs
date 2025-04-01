using SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TgUser
{
    [Key]
    public long Id { get; set; }

    [StringLength(32)]
    public string? Username { get; set; }

    [Required]
    [StringLength(64)]
    public string FirstName { get; set; }

    [StringLength(64)]
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("Group")]
    public int? GroupId { get; set; }

    public bool Subscribed { get; set; }

    public Group? Group { get; set; }
}

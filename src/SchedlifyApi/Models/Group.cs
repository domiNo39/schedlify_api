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
    
    public int AdministratorId { get; set; }

    [Required]
    public string Name { get; set; }
    
    
    [ForeignKey("AdministratorId")]
    public User Administrator { get; set; }

    // ��������� ����������
    [ForeignKey("DepartmentId")]
    public Department Department { get; set; }

    // �������� ��� ������������, �� �������� �� ���� �����
    public ICollection<TgUser> TgUsers { get; set; }

    // ���� ��������
    public ICollection<Assignment> Assignments { get; set; }
    public ICollection<Class> Classes { get; set; }
}

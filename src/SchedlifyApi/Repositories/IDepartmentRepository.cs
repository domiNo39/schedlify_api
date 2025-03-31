using SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchedlifyApi.Repositories;

public interface IDepartmentRepository
{
    Task<Department?> GetById(int departmentId);
    Task<List<Department>> GetAll(int universityId, string? s, int offset, int limit);
    Task<List<Department>> GetAll(int universityId);
    Task<Department?> GetByName(string name);
    Task<List<Department>> GetByNamePart(string name);
    Task<Department> Create(Department department);
    Task<Department> Update(Department department);
    Task Delete(int departmentId);
}
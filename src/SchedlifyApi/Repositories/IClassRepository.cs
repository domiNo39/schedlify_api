using SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchedlifyApi.Repositories;

public interface IClassRepository
{
    Task<Class?> GetByIdAsync(int id);
    Task<List<Class>> GetByGroupId(int groupId);

    Task<Class> Create(Class department);

    Task<Class> Update(Class group);
    Task Delete(int groupId);
}
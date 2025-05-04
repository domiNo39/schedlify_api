using SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchedlifyApi.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetById(int groupId);
    Task<List<Group>> GetAll(int? departmentId, int? administratorId, string? s, int offset, int limit);
    Task<List<Group>> GetAll(int departmentId);
    Task<Group?> GetByName(string name);
    Task<List<Group>> GetByNamePart(string name);
    Task<Group> Create(Group group);
    Task<Group> Update(Group group);
    Task Delete(int groupId);
}
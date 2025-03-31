using SchedlifyApi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchedlifyApi.Repositories;

public interface IUniversityRepository
{
    Task<University?> GetById(int universityId);
    Task<List<University>> GetAll(string? s, int offset, int limit);
    Task<List<University>> GetAll();
    Task<University?> GetByName(string name);
    Task<List<University>> GetByNamePart(string name);
    Task<University> Create(University university);
    Task<University> Update(University university);
    Task Delete(int universityId);
}
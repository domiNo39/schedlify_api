//COPYRIGHT NIGGERCODE
namespace SchedlifyApi.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface ITemplateSlotRepository
{
    Task<List<TemplateSlot>> GetByDepartmentIdAsync(int departmentId);

    Task<TemplateSlot?> GetByDepartmentIdAndClassNumber(int departmentId, int classNumber);
    Task CreateBulkAsync(int departmentId, List<TemplateSlot> templateSlots);
}
//COPYRIGHT NIGGERCODE
namespace SchedlifyApi.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface ITemplateSlotRepository
{
    Task<List<TemplateSlot>> GetByDepartmentIdAsync(int departmentId);
    Task CreateBulkAsync(int departmentId, List<TemplateSlot> templateSlots);
}
//COPYRIGHT NIGGERCODE
namespace SchedlifyApi.DTO;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class TemplateSlotDtos
{
    public class Response
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int ClassNumber { get; set; }
    }

    public class CreateRequest
    {
        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }

    public class BulkCreateRequest
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public List<CreateRequest> Slots { get; set; }
    }
}
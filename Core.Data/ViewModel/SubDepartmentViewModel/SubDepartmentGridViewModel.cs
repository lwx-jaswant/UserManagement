using System;

namespace Core.Data.Models.SubDepartmentViewModel
{
    public class SubDepartmentGridViewModel : EntityBase
    {
        public Int64 DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

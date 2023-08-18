﻿using System;

namespace Core.Data.Models
{
    public class SubDepartment : EntityBase
    {
        
        public Int64 DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
    }
}

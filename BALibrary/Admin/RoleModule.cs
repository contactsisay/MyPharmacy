﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Admin
{
    [Table("RoleModules")]
    public class RoleModule
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Select Role")]
        [Display(Name = "Role Name")]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Please Select Module")]
        [Display(Name = "Module Name")]
        public int ModuleId { get; set; } //from constant (enum)

        [ScaffoldColumn(false)]
        public int Status { get; set; } = 1;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }
}

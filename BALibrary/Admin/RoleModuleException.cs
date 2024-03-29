﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BALibrary.Admin
{
    [Table("RoleModuleExceptions")]
    public class RoleModuleException
    {
        [Key]
        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public int RoleModuleId { get; set; }

        [Required(ErrorMessage = "Please Select Table")]
        [Display(Name = "Table Name")]
        public string TableName { get; set; } //from module-table

        [Display(Name = "Browse")]
        public bool Browse { get; set; } = true;

        [Display(Name = "Read")]
        public bool Read { get; set; } = true;

        [Display(Name = "Edit")]
        public bool Edit { get; set; } = true;

        [Display(Name = "Add")]
        public bool Add { get; set; } = true;

        [Display(Name = "Delete")]
        public bool Delete { get; set; } = true;

        [Display(Name = "Fully Granted")]
        public bool FullyGranted { get; set; } = true;

        [Display(Name = "Fully Denied")]
        public bool FullyDenied { get; set; } = false;

        [Required(ErrorMessage = "Please Select Access Right")]
        [Display(Name = "Access Right Name")]
        public int AccessRightName { get; set; } //from constant (enum) -> granted, denied, owned & self

        [ScaffoldColumn(false)]
        public int Status { get; set; } = 1;

        [ForeignKey("RoleModuleId")]
        public virtual RoleModule RoleModule { get; set; }
    }
}

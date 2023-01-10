using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MyPharmacy.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public ApplicationDbContext()
        {
        }

        public DbSet<BALibrary.Admin.Role> Roles { get; set; }
        public DbSet<BALibrary.Admin.RoleModule> RoleModules { get; set; }
        public DbSet<BALibrary.Admin.RoleModuleException> RoleModuleExceptions { get; set; }
        public DbSet<BALibrary.Admin.ModuleTable> ModuleTables { get; set; }

        public DbSet<BALibrary.HR.Department> Departments { get; set; }
        public DbSet<BALibrary.HR.DocumentType> DocumentTypes { get; set; }
        public DbSet<BALibrary.HR.Employee> Employees { get; set; }
        public DbSet<BALibrary.HR.EmployeeAttendance> EmployeeAttendances { get; set; }
        public DbSet<BALibrary.HR.EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<BALibrary.HR.EmployeeLeave> EmployeeLeaves { get; set; }
        public DbSet<BALibrary.HR.EmployeeOffday> EmployeeOffdays { get; set; }
        public DbSet<BALibrary.HR.EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<BALibrary.HR.EmployeeShift> EmployeeShifts { get; set; }
        public DbSet<BALibrary.HR.EmployeeSocialMediaLink> EmployeeSocialMediaLinks { get; set; }
        public DbSet<BALibrary.HR.EmployeeTransfer> EmployeeTransfers { get; set; }
        public DbSet<BALibrary.HR.EmploymentType> EmploymentTypes { get; set; }
        public DbSet<BALibrary.HR.JobPosition> JobPositions { get; set; }
        public DbSet<BALibrary.HR.LeaveType> LeaveTypes { get; set; }
        public DbSet<BALibrary.HR.ShiftType> ShiftTypes { get; set; }
        public DbSet<BALibrary.HR.SocialMedia> SocialMedias { get; set; }
        public DbSet<BALibrary.HR.UserType> UserTypes { get; set; }

        public DbSet<BALibrary.Inventory.ManualCount> ManualCounts { get; set; }
        public DbSet<BALibrary.Inventory.Product> Products { get; set; }
        public DbSet<BALibrary.Inventory.ProductBatch> ProductBatches { get; set; }
        public DbSet<BALibrary.Inventory.ProductCategory> ProductCategories { get; set; }
        public DbSet<BALibrary.Inventory.Stock> Stocks { get; set; }
        public DbSet<BALibrary.Inventory.Uom> Uoms { get; set; }

        public DbSet<BALibrary.Purchase.Supplier> Suppliers { get; set; }
        public DbSet<BALibrary.Purchase.PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<BALibrary.Sales.Customer> Customers { get; set; }
        public DbSet<BALibrary.Sales.CustomerType> CustomerTypes { get; set; }
        public DbSet<BALibrary.Sales.Invoice> Invoices { get; set; }
        public DbSet<BALibrary.Sales.InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<BALibrary.Sales.InvoiceType> InvoiceTypes { get; set; }

        public DbSet<BALibrary.Report.ProfitLossReport> ProfitLossReports { get; set; }
        public DbSet<BALibrary.Report.PurchaseReport> PurchaseReports { get; set; }
        public DbSet<BALibrary.Report.SalesReport> SalesReports { get; set; }
        public DbSet<BALibrary.Report.StockReport> StockReports { get; set; }
    }
}
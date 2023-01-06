using BALibrary.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
namespace MyPharmacy.Models
{
    public class Common
    {
        public static decimal GetProductBatchBalance(int ProductBatchId)
        {
            decimal returnValue = 0;
            string query = "SELECT * FROM Stocks WHERE Id = " + ProductBatchId + " ORDER BY Id DESC";
            using var sqlConnection = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = sqlConnection };
            sqlConnection.OpenAsync();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                returnValue += Convert.ToDecimal(reader["CurrentQuantity"].ToString());
                break;
            }
            return returnValue;
        }

        public static int GetObjectIdFromName(string table_name, string field_name, string field_value)
        {
            int id = 0;
            string query = "SELECT * FROM " + table_name + " WHERE " + field_name + " LIKE '%" + field_value + "%'";
            using var sqlConnection = new SqlConnection(GetConfiguration().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            using var cmd = new SqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = sqlConnection };
            sqlConnection.OpenAsync();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                id = Convert.ToInt32(reader["id"].ToString());
                break;
            }
            return id;
        }

        public static bool CheckForObjectExistance(string table_name, string field_name, string field_value)
        {
            bool item_exists = false;
            string query = "SELECT * FROM " + table_name + " WHERE " + field_name + " = '" + field_value + "'";
            using var sqlConnection = new SqlConnection(GetConfiguration().GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            using var cmd = new SqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = sqlConnection };
            sqlConnection.OpenAsync();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                item_exists = true;
                break;
            }

            return item_exists;
        }

        public static List<SelectListItem> GetTableComboBox()
        {
            List<string> fields = new List<string>();
            fields.Add("TABLE_NAME");
            fields.Add("TABLE_NAME");
            string condition = " TABLE_TYPE = 'Base Table' and TABLE_NAME NOT LIKE '%AspNet%' AND TABLE_NAME NOT LIKE '%__EFMigrationsHistory%'";
            List<SelectListItem> list = GetCustomSelectList("INFORMATION_SCHEMA.TABLES", fields, condition);

            return list;
        }

        public static List<string> GetTableNames()
        {
            List<string> items = new List<string>();
            string condition = " TABLE_TYPE = 'Base Table' and TABLE_NAME NOT LIKE '%AspNet%' AND TABLE_NAME NOT LIKE '%__EFMigrationsHistory%'";

            string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE " + condition + " ORDER BY TABLE_NAME DESC";
            using var sqlConnection = new SqlConnection(Common.ConnectionString);
            using var cmd = new SqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = sqlConnection };
            sqlConnection.OpenAsync();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(Convert.ToString(reader["TABLE_NAME"]));
            }

            return items;
        }

        public static List<SelectListItem> GetCustomSelectList(string table_name, List<string> fields, string condition = "1=1")
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Value = "-1";
            item.Text = "--Select--";
            list.Add(item);

            if (table_name != null && table_name != "" && fields.Count() >= 2)
            {
                string query = "SELECT ";
                int i = 0;

                #region Adding Fields to Query String
                foreach (string a in fields)
                {
                    if (i < (fields.Count() - 1))
                        query += " " + a + ",";
                    else
                        query += a;
                    i++;
                }
                #endregion

                query += " FROM " + table_name + " WHERE " + condition + " ORDER BY " + fields[0] + " DESC";
                using var sqlConnection = new SqlConnection(Common.ConnectionString);
                using var cmd = new SqlCommand() { CommandText = query, CommandType = CommandType.Text, Connection = sqlConnection };
                sqlConnection.OpenAsync();

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    item = new SelectListItem();
                    item.Value = Convert.ToString(reader[fields[0]]);
                    item.Text = Convert.ToString(reader[fields[1]]);
                    list.Add(item);
                }
            }

            return list;
        }

        public static List<SelectListItem> GetYearsComboBox()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = null;
            for (int i = DateTime.Now.Year; i >= 1999; i--)
            {
                item = new SelectListItem();
                item.Value = i.ToString();
                item.Text = i.ToString();
                list.Add(item);
            }

            return list;
        }

        public static List<SelectListItem> GetWeeksComboBox()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = null;
            for (int i = Common.GetWeekNoFromDate(DateTime.Now); i >= 1; i--)
            {
                item = new SelectListItem();
                item.Value = i.ToString();
                item.Text = i.ToString();
                list.Add(item);
            }

            return list;
        }

        public static int GetWeekNoFromDate(DateTime now)
        {
            CultureInfo cultureInfo = CultureInfo.CurrentCulture;
            return cultureInfo.Calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static List<DateTime> GetWeekDatesFromDate(DateTime dte)
        {
            int currentDayOfWeek = (int)dte.DayOfWeek;
            DateTime sunday = dte.AddDays(-currentDayOfWeek);
            DateTime monday = sunday.AddDays(1);
            if (currentDayOfWeek == 0)
                monday = monday.AddDays(-7);
            List<DateTime> dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
            return dates;
        }

        public static string HighlightKeywords(string input, string keywords)
        {
            if (input == string.Empty || keywords == string.Empty)
            {
                return input;
            }

            string[] sKeywords = keywords.Split(' ');
            foreach (string sKeyword in sKeywords)
            {
                try
                {
                    input = Regex.Replace(input, sKeyword, string.Format("<span style='background-color:lightgreen;'>{0}</span>", "$0"), RegexOptions.IgnoreCase);
                }
                catch
                {
                }
            }
            return input;
        }

        public static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }
        private static string ConnectionString
        {
            get
            {
                return GetConfiguration().GetConnectionString("DefaultConnection");
            }
        }

        /* CONSTANT and OTHER DECLARATIONS START */

        #region CONSTANT DECLARATIONS
        public enum ModuleName
        {
            ADMIN = 1,
            DASHBOARD = 2,
            HR = 3,
            INVENTORY = 4,
            PURCHASE = 5,
            SALES = 6,
        };

        public enum StockEntry
        {
            IMPORTED = 1,
            SIV = 2,
            GRN = 3,
        };

        #endregion

        #region STOCK ENTRY RELATED
        public static string GetStockEntryText(StockEntry stock_entry)
        {
            string name = string.Empty;
            if (stock_entry == StockEntry.IMPORTED)
                name = StockEntry.IMPORTED.ToString();
            else if (stock_entry == StockEntry.SIV)
                name = StockEntry.SIV.ToString();
            else if (stock_entry == StockEntry.GRN)
                name = StockEntry.GRN.ToString();

            return name;
        }

        public static StockEntry GetStockEntryId(string stock_entry)
        {
            StockEntry name = StockEntry.IMPORTED;
            if (stock_entry == StockEntry.IMPORTED.ToString())
                name = StockEntry.IMPORTED;
            else if (stock_entry == StockEntry.SIV.ToString())
                name = StockEntry.SIV;
            else if (stock_entry == StockEntry.GRN.ToString())
                name = StockEntry.GRN;

            return name;
        }
        #endregion

        #region MODULE RELATED

        public static string GetModuleText(ModuleName module_name)
        {
            string name = string.Empty;
            if (module_name == ModuleName.ADMIN)
                name = ModuleName.ADMIN.ToString();
            else if (module_name == ModuleName.DASHBOARD)
                name = ModuleName.DASHBOARD.ToString();
            else if (module_name == ModuleName.HR)
                name = ModuleName.HR.ToString();
            else if (module_name == ModuleName.INVENTORY)
                name = ModuleName.INVENTORY.ToString();
            else if (module_name == ModuleName.PURCHASE)
                name = ModuleName.PURCHASE.ToString();
            else if (module_name == ModuleName.SALES)
                name = ModuleName.SALES.ToString();

            return name;
        }

        public static ModuleName GetModuleId(string module_name)
        {
            ModuleName name = ModuleName.ADMIN;
            if (module_name == ModuleName.ADMIN.ToString())
                name = ModuleName.ADMIN;
            else if (module_name == ModuleName.DASHBOARD.ToString())
                name = ModuleName.DASHBOARD;
            else if (module_name == ModuleName.HR.ToString())
                name = ModuleName.HR;
            else if (module_name == ModuleName.INVENTORY.ToString())
                name = ModuleName.INVENTORY;
            else if (module_name == ModuleName.PURCHASE.ToString())
                name = ModuleName.PURCHASE;
            else if (module_name == ModuleName.SALES.ToString())
                name = ModuleName.SALES;

            return name;
        }

        public static List<SelectListItem> FillModuleComboBox(string key)
        {
            List<SelectListItem> collection = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            string[] names = Enum.GetNames(typeof(ModuleName));
            foreach (string name in names)
            {
                if (string.IsNullOrEmpty(key) && name.ToLower().Contains(key.ToLower()))
                {
                    item = new SelectListItem();
                    item.Value = name;
                    item.Text = name;
                    collection.Add(item);
                }
            }

            return collection;
        }

        public static string[] GetModuleNames()
        {
            string[] names = Enum.GetNames(typeof(ModuleName));
            return names;
        }

        public static bool CheckModuleExistance(List<RoleModule> userRoleModules, int moduleId)
        {
            bool status = false;
            foreach (RoleModule roleModule in userRoleModules)
            {
                if (roleModule.ModuleId == moduleId)
                {
                    status = true;
                    break;
                }
            }
            return status;
        }

        #endregion

        /* CONSTANT and OTHER DECLARATIONS END */
    }
}

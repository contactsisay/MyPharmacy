using BALibrary.Inventory;
using BALibrary.Purchase;
using BALibrary.Purchase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Data;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Purchase.Controllers
{
    [Area("Purchase")]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Purchase/PurchaseOrders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PurchaseOrders.Include(p => p.Product).Include(p => p.Supplier);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Purchase/PurchaseOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // GET: Purchase/PurchaseOrders/Create
        public IActionResult Create()
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");

            #region Preparing parameters
            var products = _context.Products.Include(p => p.Uom);
            var suppliers = _context.Suppliers;
            var pOptions = string.Empty;
            var sOptions = string.Empty;
            foreach (Product p in products)
            {
                pOptions += p.Id + "#" + (p.Name + "(" + p.Code + ")") + "#" + p.Uom.Name + "#" + p.MinimumOrderLevel + ",";//id#(name+code)#uom-name#minimum-order-level
            }
            foreach (Supplier s in suppliers)
            {
                sOptions += s.Id + "#" + (s.Name + "(" + s.ContactPersonName + ")") + "#" + s.ContactPersonMobile + "#" + s.Address + ",";//id#(name+contact-person-name)#contact-person-mobile#supplier-address
            }
            #endregion

            ViewData["pOptions"] = pOptions;
            ViewData["sOptions"] = sOptions;
            return View();
        }

        // POST: Purchase/PurchaseOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,SupplierId,RequiredAmount,ApprovedAmount,SupplierInvoiceNo")] PurchaseOrder purchaseOrder)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                }
                else
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePR(PurchaseOrderViewModel purchaseOrderViewModel, IFormCollection iformCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            ProductBatch productBatch = new ProductBatch();
            PurchaseOrder purchaseOrder = new PurchaseOrder();

            for (int i = 0; i < iformCollection.Count; i++)
            {
                if (!string.IsNullOrEmpty(iformCollection["productid_" + i]))
                {
                    productBatch = new ProductBatch();
                    productBatch.ProductId = Convert.ToInt32(iformCollection["productid_" + i].ToString());
                    if (!string.IsNullOrEmpty(iformCollection["batch_no_" + i]))
                        productBatch.BatchNo = iformCollection["batch_no_" + i].ToString();
                    else
                        break;

                    productBatch.ManufacturedDate = Convert.ToDateTime(iformCollection["manufactured_date_" + i].ToString());
                    productBatch.BestBefore = Convert.ToDateTime(iformCollection["best_before_" + i].ToString());
                    productBatch.ExpirationDate = Convert.ToDateTime(iformCollection["expiry_date_" + i].ToString());
                    productBatch.PurchasingPrice = Convert.ToDecimal(iformCollection["purchasing_price_" + i].ToString());
                    productBatch.SellingPrice = Convert.ToDecimal(iformCollection["selling_price_" + i].ToString());
                    productBatch.IsSellable = (iformCollection["is_sellable_" + i].ToString() == "on" ? true : false);
                    productBatch.IsTaxable = (iformCollection["istaxable_" + i].ToString() == "on" ? true : false);
                    productBatch.EmployeeId = currentUserId;
                    productBatch.Status = 1;

                    purchaseOrder = new PurchaseOrder();
                    if (!string.IsNullOrEmpty(iformCollection["supplierid_" + i]))
                        purchaseOrder.SupplierId = Convert.ToInt32(iformCollection["supplierid_" + i].ToString());
                    else
                        break;

                    purchaseOrder.ProductId = productBatch.ProductId;
                    purchaseOrder.SupplierInvoiceNo = (string.IsNullOrEmpty(iformCollection["invoice_no"]) ? string.Empty : iformCollection["invoice_no"].ToString());
                    purchaseOrder.RequiredAmount = Convert.ToInt32(iformCollection["quantity_" + i].ToString());
                    purchaseOrder.ApprovedAmount = Convert.ToInt32(iformCollection["quantity_" + i].ToString());
                    purchaseOrder.RequestedAt = DateTime.Now;
                    purchaseOrder.RequestedBy = currentUserId;
                    purchaseOrder.ApprovedBy = currentUserId;
                    purchaseOrder.ApprovedAt = DateTime.Now;
                    purchaseOrder.Status = 1;

                    #region Saving the details

                    //Purchase Order
                    int poId = 0;
                    if (purchaseOrder != null)
                    {
                        purchaseOrder.RequiredAmount = purchaseOrder.ApprovedAmount;
                        purchaseOrder.RequestedBy = currentUserId;
                        purchaseOrder.RequestedAt = DateTime.Now;
                        purchaseOrder.ProductId = productBatch.ProductId;

                        _context.Add(purchaseOrder);
                        poId = await _context.SaveChangesAsync();
                    }

                    //Product Batch
                    int pbId = 0;
                    if (poId > 0 && productBatch != null)
                    {
                        productBatch.PurchaseOrderId = purchaseOrder.Id;
                        productBatch.EmployeeId = currentUserId;
                        productBatch.Status = 1;

                        _context.Add(productBatch);
                        pbId = await _context.SaveChangesAsync();
                    }

                    //Stock
                    int sId = 0;
                    if (poId > 0 && pbId > 0)
                    {
                        Stock stock = new Stock();
                        stock.ProductBatchId = productBatch.Id;
                        stock.InitialQuantity = purchaseOrder.ApprovedAmount;
                        stock.SoldQuantity = 0;
                        stock.CurrentQuantity = purchaseOrder.ApprovedAmount;
                        stock.ActionTaken = 0;//purchase
                        stock.Description = "Entered from Purchase Order";
                        stock.Status = 0;
                        stock.EmployeeId = currentUserId;
                        stock.UpdatedAt = DateTime.Now;

                        _context.Stocks.Add(stock);
                        int pass = await _context.SaveChangesAsync();

                        if (pass > 0)
                        {
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Saved Successfully!");
                        }
                        else
                        {
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                            HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Saved!");
                        }
                    }
                    #endregion
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", purchaseOrder.SupplierId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Purchase/PurchaseOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);

            var productBatch = _context.ProductBatches.Where(pb => pb.PurchaseOrderId == purchaseOrder.Id).FirstOrDefault();
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.PurchaseOrder = purchaseOrder;
            purchaseOrderViewModel.ProductBatch = productBatch;

            return View(purchaseOrderViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPR(PurchaseOrderViewModel purchaseOrderViewModel)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            ProductBatch productBatch = purchaseOrderViewModel.ProductBatch;
            PurchaseOrder purchaseOrder = purchaseOrderViewModel.PurchaseOrder;

            //Purchase Order
            int poId = 0;
            if (purchaseOrder != null)
            {
                purchaseOrder.RequiredAmount = purchaseOrder.ApprovedAmount;
                purchaseOrder.RequestedBy = currentUserId;
                purchaseOrder.RequestedAt = DateTime.Now;
                purchaseOrder.ProductId = productBatch.ProductId;

                _context.Update(purchaseOrder);
                poId = await _context.SaveChangesAsync();
            }

            //Product Batch
            int pbId = 0;
            if (poId > 0 && productBatch != null)
            {
                productBatch.PurchaseOrderId = purchaseOrder.Id;
                productBatch.EmployeeId = currentUserId;

                _context.Update(productBatch);
                pbId = await _context.SaveChangesAsync();
            }

            //Stock
            int sId = 0;
            if (poId > 0 && pbId > 0)
            {
                Stock stock = new Stock();
                stock.ProductBatchId = productBatch.Id;
                stock.InitialQuantity = purchaseOrder.ApprovedAmount;
                stock.SoldQuantity = purchaseOrder.ApprovedAmount;
                stock.CurrentQuantity = purchaseOrder.ApprovedAmount;
                stock.ProductBatchId = pbId;
                stock.ActionTaken = 0;//purchase
                stock.Description = "Entered from Purchase Order";
                stock.Status = 0;
                stock.EmployeeId = currentUserId;
                stock.UpdatedAt = DateTime.Now;

                _context.Stocks.Update(stock);
                int pass = await _context.SaveChangesAsync();

                if (pass > 0)
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                }
                else
                {
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                    HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Updated!");
                }
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return RedirectToAction(nameof(Index));
        }

        // POST: Purchase/PurchaseOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,SupplierId,RequiredAmount,ApprovedAmount,SupplierInvoiceNo")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    int pass = await _context.SaveChangesAsync();

                    if (pass > 0)
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Updated Successfully!");
                    }
                    else
                    {
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                        HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Updated!");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", purchaseOrder.ProductId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Address", purchaseOrder.SupplierId);
            return View(purchaseOrder);
        }

        // GET: Purchase/PurchaseOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessageType);
            HttpContext.Session.Remove(SessionVariable.SessionKeyMessage);

            if (id == null || _context.PurchaseOrders == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(p => p.Product)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        // POST: Purchase/PurchaseOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PurchaseOrders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PurchaseOrders'  is null.");
            }
            var purchaseOrder = await _context.PurchaseOrders.FindAsync(id);
            if (purchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(purchaseOrder);
            }

            int pass = await _context.SaveChangesAsync();

            if (pass > 0)
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "success");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " Deleted Successfully!");
            }
            else
            {
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessageType, "error");
                HttpContext.Session.SetString(SessionVariable.SessionKeyMessage, this.ControllerContext.RouteData.Values["controller"].ToString().ToUpper() + " NOT Deleted!");
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }
    }
}

﻿using BALibrary.Inventory;
using BALibrary.Sales;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyPharmacy.Data;
using MyPharmacy.Models;

namespace MyPharmacy.Areas.Sales.Controllers
{
    [Area("Sales")]
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sales/Invoices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invoices.Include(i => i.Customer).Include(i => i.InvoiceType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Sales/Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            var invoiceDetails = _context.InvoiceDetails.Where(idd => idd.InvoiceId == invoice.Id);
            ViewData["invoiceDetails"] = invoiceDetails;
            return View(invoice);
        }

        // GET: Sales/Invoices/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["InvoiceTypeId"] = new SelectList(_context.InvoiceTypes, "Id", "Name");

            var productBatches = _context.ProductBatches.Include(pb => pb.Product);
            var pbOptions = string.Empty;
            foreach (ProductBatch pb in productBatches)
            {
                List<Stock> stocks = _context.Stocks.Where(s => s.ProductBatchId == pb.Id).OrderByDescending(pb => pb.Id).ToList();
                pbOptions += pb.Id + "#" + (pb.Product.Name + "(" + pb.BatchNo + ")") + "#" + pb.SellingPrice + "#" + (stocks.Count > 0 ? stocks[0].CurrentQuantity : 0) + "#" + (pb.IsTaxable ? 1 : 0) + ",";//id#(product_name+batch_no)#selling_price#stock_balance#is_taxable
            }

            string nextInvoiceNo = string.Empty;
            List<Invoice> invoices = _context.Invoices.OrderByDescending(inv => inv.Id).ToList();
            nextInvoiceNo = (invoices.Count > 0 ? (Convert.ToInt32(invoices[0].InvoiceNo) + 1).ToString() : "1");
            ViewData["NextInvoiceNo"] = nextInvoiceNo;
            ViewData["pbOptions"] = pbOptions;
            return View();
        }

        public async Task<IActionResult> PrintInvoice(int id)
        {
            Invoice invoice = _context.Invoices.Find(id);
            List<InvoiceDetail> invoiceDetails = _context.InvoiceDetails.Where(idd => idd.InvoiceId == id).ToList();
            ViewData["Invoice"] = invoice;
            ViewData["InvoiceDetails"] = invoiceDetails;
            return View();
        }

        public async Task<IActionResult> GenerateInvoice(int id)
        {
            Invoice invoice = _context.Invoices.Find(id);
            List<InvoiceDetail> invoiceDetails = _context.InvoiceDetails.Where(idd => idd.InvoiceId == id).ToList();
            ViewData["Invoice"] = invoice;
            ViewData["InvoiceDetails"] = invoiceDetails;
            return View();
        }

        // POST: Sales/Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,InvoiceTypeId,InvoiceNo,InvoiceTotal,InvoiceDate")] Invoice invoice, IFormCollection formCollection)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            invoice.InvoiceTotal = 0;
            invoice.InvoiceDate = DateTime.Now;
            invoice.EmployeeId = currentUserId;
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            if (ModelState.IsValid)
            {
                //Saving Invoice
                int pass = 0;
                _context.Add(invoice);
                pass = await _context.SaveChangesAsync();

                #region Saving Invoice Details
                //Saving Invoice Details
                if (pass > 0)
                {
                    for (int i = 1; i <= formCollection.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(formCollection["productbatchid_" + i].ToString()) && Convert.ToInt32(formCollection["productbatchid_" + i].ToString()) > 0)
                        {
                            int productBatchId = Convert.ToInt32(formCollection["productbatchid_" + i].ToString());
                            ProductBatch productBatch = _context.ProductBatches.Find(productBatchId);
                            if (productBatch != null)
                            {
                                InvoiceDetail invoiceDetail = new InvoiceDetail();
                                invoiceDetail.InvoiceId = invoice.Id;
                                invoiceDetail.ProductBatchId = Convert.ToInt32(formCollection["productbatchid_" + i]);
                                invoiceDetail.Quantity = Convert.ToInt32(formCollection["quantity_" + i]);
                                invoiceDetail.SellingPrice = Convert.ToDecimal(formCollection["unitprice_" + i]);

                                decimal rowTotal = invoiceDetail.Quantity * invoiceDetail.SellingPrice;
                                if (productBatch.IsTaxable == true)
                                    invoiceDetail.RowTotal = rowTotal + (rowTotal * Convert.ToDecimal(0.15)); //getting 15% tax
                                else
                                    invoiceDetail.RowTotal = rowTotal;

                                invoiceDetail.Status = 1;

                                _context.InvoiceDetails.Add(invoiceDetail);
                                int pass2 = await _context.SaveChangesAsync();

                                //if saving invoice detail is successful, then deduct from stock
                                if (pass2 > 0)
                                {
                                    List<Stock> stocks = _context.Stocks.Where(s => s.ProductBatchId == invoiceDetail.ProductBatchId).OrderByDescending(s => s.Id).ToList();
                                    if (stocks.Count > 0)
                                    {
                                        Stock s = stocks[0];
                                        s.EmployeeId = currentUserId;
                                        s.SoldQuantity = invoiceDetail.Quantity;
                                        s.CurrentQuantity = s.CurrentQuantity - invoiceDetail.Quantity;
                                        s.ActionTaken = 1;//sold
                                        s.Description = "Sold by Invoice No: " + invoice.InvoiceNo;
                                        s.Status = 1;

                                        _context.Stocks.Update(s);
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion

                return RedirectToAction("GenerateInvoice", new { id = invoice.Id });
            }

            return RedirectToAction("GenerateInvoice", new { id = invoice.Id });
        }

        [HttpPost()]
        public ActionResult CheckStockBalance(string id)
        {
            //bool pass = false;


            return Ok(id);
        }

        // GET: Sales/Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", invoice.CustomerId);
            ViewData["InvoiceTypeId"] = new SelectList(_context.InvoiceTypes, "Id", "Name", invoice.InvoiceTypeId);
            return View(invoice);
        }

        public async Task<IActionResult> VoidInvoice(int id)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                //updating stock
                var invoiceDetails = _context.InvoiceDetails.Where(i => i.InvoiceId == id);
                foreach (InvoiceDetail invDetail in invoiceDetails)
                {
                    var invoiceDetail = await _context.InvoiceDetails.FindAsync(invDetail.Id);
                    if (invoiceDetail != null)
                    {
                        //updating in stock
                        List<Stock> stocks = _context.Stocks.Where(s => s.ProductBatchId == invoiceDetail.ProductBatchId).OrderByDescending(s => s.Id).ToList();
                        if (stocks.Count > 0)
                        {
                            Stock stock = stocks[0];
                            stock.SoldQuantity -= invoiceDetail.Quantity;
                            stock.CurrentQuantity += invoiceDetail.Quantity;
                            stock.ActionTaken = 3; //reversed - void invoice
                            stock.Description = "Invoice No: " + invoice.InvoiceNo + " is made VOID";
                            stock.Status = 1;
                            stock.EmployeeId = currentUserId;
                            stock.UpdatedAt = DateTime.Now;

                            //updating stock balance
                            _context.Stocks.Update(stock);
                        }
                    }
                }

                invoice.Status = 0;//in-active
                _context.Invoices.Update(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Sales/Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,InvoiceTypeId,InvoiceNo,InvoiceTotal,InvoiceDate")] Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", invoice.CustomerId);
            ViewData["InvoiceTypeId"] = new SelectList(_context.InvoiceTypes, "Id", "Name", invoice.InvoiceTypeId);
            return View(invoice);
        }

        // GET: Sales/Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Invoices == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.InvoiceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Sales/Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int currentUserId = 1;//default admin account id
            if (HttpContext.Session.GetString(SessionVariable.SessionKeyUserId) != null)
                currentUserId = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.SessionKeyUserId));

            if (_context.Invoices == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Invoices'  is null.");
            }
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                var invoiceDetails = _context.InvoiceDetails.Where(i => i.InvoiceId == id);
                //removing invoice details
                foreach (InvoiceDetail invDetail in invoiceDetails)
                {
                    var invoiceDetail = await _context.InvoiceDetails.FindAsync(invDetail.Id);
                    if (invoiceDetail != null)
                    {
                        //updating in stock
                        List<Stock> stocks = _context.Stocks.Where(s => s.ProductBatchId == invoiceDetail.ProductBatchId).OrderByDescending(s => s.Id).ToList();
                        if (stocks.Count > 0)
                        {
                            Stock stock = stocks[0];
                            stock.SoldQuantity -= invoiceDetail.Quantity;
                            stock.CurrentQuantity += invoiceDetail.Quantity;
                            stock.ActionTaken = 2; //reversed
                            stock.Description = "Sales with Invoice No: " + invoice.InvoiceNo + " is REVERSED";
                            stock.Status = 1;
                            stock.EmployeeId = currentUserId;
                            stock.UpdatedAt = DateTime.Now;

                            //updating stock balance
                            _context.Stocks.Update(stock);
                        }

                        //removing from invoice details
                        _context.InvoiceDetails.Remove(invoiceDetail);
                    }
                }

                //removing invoices
                _context.Invoices.Remove(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.Id == id);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FileHelpers;
using CAPCO.Areas.Admin.Models;
using CAPCO.Infrastructure.Data;
using CAPCO.Infrastructure.Domain;
using System.Text;
using CAPCO.Infrastructure.Services;
using CAPCO.Models;

namespace CAPCO.Areas.Admin.Controllers
{
    public class ImportController : BaseAdminController
    {
        private readonly IApplicationUserService _CustomerService;
        private readonly IRepository<Product> _ProductRepository;
        private readonly IRepository<ProductGroup> _ProductgroupRepository;
        private readonly IRepository<ProductCategory> _ProductcategoryRepository;
        private readonly IRepository<ProductType> _ProducttypeRepository;
        private readonly IRepository<ProductColor> _ProductcolorRepository;
        private readonly IRepository<ProductSize> _ProductsizeRepository;
        private readonly IRepository<ProductFinish> _ProductfinishRepository;
        private readonly IRepository<ProductUnitOfMeasure> _ProductUomRepository;
        private readonly IRepository<Manufacturer> _ManufacturerRepository;
        private readonly IRepository<ProductUsage> _ProductUsageRepository;
        private readonly IRepository<ProductVariation> _ProductVariationRepository;
        private readonly IRepository<ProductStatus> _ProductStatusRepository;
        private readonly IRepository<PriceCode> _PriceCodeRepo;
        private readonly IRepository<RelatedProductSize> _OtherSizeRepo;
        private readonly IRepository<RelatedAccent> _AccentRepo;
        private readonly IRepository<RelatedTrim> _TrimRepo;
        private readonly IRepository<ProductPriceCode> _ProductPriceCodeRepo;
        private readonly IRepository<ProductSeries> _ProductSeriesRepo;
        private readonly IRepository<PickupLocation> _LocationRepo;
        private readonly IRepository<DiscountCode> _DiscountCodeRepo;
        private readonly IRepository<ApplicationUser> _AppUserRepo; 


        public ImportController(IRepository<Product> productRepository, 
            IRepository<ProductGroup> productgroupRepository,
            IRepository<ProductCategory> productcategoryRepository,
            IRepository<ProductType> producttypeRepository,
            IRepository<ProductColor> productcolorRepository,
            IRepository<ProductSize> productsizeRepository,
            IRepository<ProductFinish> productfinishRepository,
            IRepository<ProductUnitOfMeasure> productUomRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<ProductUsage> productUsageRepository,
            IRepository<ProductVariation> productVariationRepository,
            IRepository<ProductStatus> productStatusRepository,
            IRepository<PriceCode> priceCodeRepo,
            IRepository<RelatedProductSize> otherSizeRepo,
            IRepository<RelatedAccent> accentRepo,
            IRepository<RelatedTrim> trimRepo, 
            IRepository<ProductPriceCode> productPriceCodeRepo,
            IRepository<ProductSeries> productSeriesRepo,
            IApplicationUserService customerService,
            IRepository<PickupLocation> locationRepo,
            IRepository<DiscountCode> discountCodeRepo,
            IRepository<ApplicationUser> appUserRepo)
        {
            _ProductSeriesRepo = productSeriesRepo;
            _ProductPriceCodeRepo = productPriceCodeRepo;
            _TrimRepo = trimRepo;
            _AccentRepo = accentRepo;
            _OtherSizeRepo = otherSizeRepo;
            _PriceCodeRepo = priceCodeRepo;
            _ProductStatusRepository = productStatusRepository;
            _ProductVariationRepository = productVariationRepository;
            _ProductUsageRepository = productUsageRepository;
            _ManufacturerRepository = manufacturerRepository;
            _ProductUomRepository = productUomRepository;
            _ProductfinishRepository = productfinishRepository;
            _ProductsizeRepository = productsizeRepository;
            _ProductcolorRepository = productcolorRepository;
            _ProducttypeRepository = producttypeRepository;
            _ProductcategoryRepository = productcategoryRepository;
            _ProductgroupRepository = productgroupRepository;
            _ProductRepository = productRepository;
            _CustomerService = customerService;
            _LocationRepo = locationRepo;
            _DiscountCodeRepo = discountCodeRepo;
            _AppUserRepo = appUserRepo;
        }
        
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Customers(HttpPostedFileBase customersFile)
        {
            try
            {
                if (customersFile == null || customersFile.ContentLength <= 0 || Path.GetExtension(customersFile.FileName).ToLower() != ".txt")
                    throw new Exception("You must provide a valid tab delimited txt file.");

                string importPath = Server.MapPath("~/Public/Imports");
                if (!Directory.Exists(importPath))
                {
                    var di = Directory.CreateDirectory(importPath);
                    // TODO: permissions?
                }

                var storedFilePath = importPath + "\\" + Path.GetFileName(customersFile.FileName);
                customersFile.SaveAs(storedFilePath);

                var engine = new FileHelperEngine(typeof(ImportedCustomer));
                var importedCustomers = engine.ReadFile(storedFilePath) as ImportedCustomer[];

                int newCustomers = 0;
                UserRoles role = UserRoles.ServiceProviders;

                var invalidUsers = new List<ImportedCustomer>();
                foreach (var item in importedCustomers)
                {
                    var registerModel = new RegisterModel();
                    registerModel.CompanyName = item.CompanyName;
                    registerModel.ConfirmPassword = item.Password;
                    registerModel.Email = item.Email;
                    registerModel.FirstName = "";// item.FirstName;
                    registerModel.LastName = "";// item.LastName;
                    registerModel.Password = item.Password;
                    registerModel.UserName = item.UserName;
                    
                    ApplicationUser customer = null;
                    try
                    {
                        customer = _CustomerService.CreateNewUser(registerModel, role);    
                    }
                    catch (Exception ex)
                    {
                        invalidUsers.Add(item);
                        continue;
                    }
                    
                    customer.AccountNumber = item.AccountNumber;
                    customer.CanReceiveMarketingEmails = item.OptInMarketing;
                    customer.CanReceiveSystemEmails = item.OptInSystem;
                    customer.City = item.City;
                    customer.Fax = item.Fax;
                    customer.HasRequestedAccount = true;
                    customer.IsActivated = true;
                    customer.Phone = item.Phone;
                    customer.State = item.State;
                    customer.StreetAddressLine1 = item.StreetAddressLine1;
                    customer.StreetAddressLine2 = item.StreetAddressLine2;
                    customer.Zip = item.ZipCode;
                    
                    customer.Status = AccountStatus.Active.ToString();
                    customer.PricePreference = "CostOnly";
                    if (!String.IsNullOrWhiteSpace(item.DefaultLocationId))
                    {
                        var location = _LocationRepo.All.FirstOrDefault(x => x.Name == item.DefaultLocationId);
                        if (location != null)
                            customer.DefaultLocation = location;
                    }

                    int code = 0;
                    if (Int32.TryParse(item.DefaultDiscountCode, out code))
                    {
                        var discountCode = _DiscountCodeRepo.All.FirstOrDefault(x => x.Code == code);
                        if (discountCode != null)
                            customer.DiscountCode = discountCode;
                    }

                    try
                    {
                        _AppUserRepo.InsertOrUpdate(customer);
                        _AppUserRepo.Save();
                    }
                    catch (Exception ex)
                    {
                        invalidUsers.Add(item);
                        _CustomerService.DeleteMember(item.UserName);
                        continue;
                    }

                    newCustomers++;
                }

                System.IO.File.Delete(storedFilePath);

                if (invalidUsers.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    invalidUsers.ForEach(x =>
                    {
                        sb.Append(x.UserName + ",");
                    });
                    this.FlashError(String.Format("{0} customer(s) were added. The following User records were invalid: {1}", newCustomers, sb.ToString()));
                }
                else
                {
                    this.FlashInfo(String.Format("{0} customer(s) were added.", newCustomers));
                }
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem importing the customer list: " + ex.Message);
            }

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Products(HttpPostedFileBase productsFile)
        {
            try
            {
                if (productsFile == null || productsFile.ContentLength <= 0 || Path.GetExtension(productsFile.FileName).ToLower() != ".txt")
                    throw new Exception("You must provide a valid tab delimited txt file.");

                string importPath = Server.MapPath("~/Public/Imports");
                if (!Directory.Exists(importPath))
                {
                    var di = Directory.CreateDirectory(importPath);
                    // TODO: permissions?
                }
                var storedFilePath = importPath + "\\" + Path.GetFileName(productsFile.FileName);
                productsFile.SaveAs(storedFilePath);

                var engine = new FileHelperEngine(typeof(ImportedProduct));
                ImportedProduct[] importedProducts = engine.ReadFile(storedFilePath) as ImportedProduct[];

                int newProducts = 0;
                int updatedProducts = 0;

                foreach (var item in importedProducts)
                {
                    if (!String.IsNullOrEmpty(item.ItemNumber))
                    { 
                        // does item exist?
                        if (_ProductRepository.All.Any(x => x.ItemNumber.ToLower() == item.ItemNumber.ToLower()))
                        {
                            // do update import
                            var productToUpdate = _ProductRepository.All.FirstOrDefault(x => x.ItemNumber.ToLower() == item.ItemNumber.ToLower());
                            ImportItemRecord(item, productToUpdate);
                            _ProductRepository.InsertOrUpdate(productToUpdate);
                            _ProductRepository.Save();
                            updatedProducts++;
                        }
                        else
                        { 
                            // do new import
                            var newProduct = new Product();
                            ImportItemRecord(item, newProduct);
                            newProduct.CreatedBy = CurrentUser.UserName;
                            newProduct.CreatedOn = DateTime.Now;
                            _ProductRepository.InsertOrUpdate(newProduct);
                            _ProductRepository.Save();
                            newProducts++;
                        }
                    }
                }

                System.IO.File.Delete(storedFilePath);
                this.FlashInfo(String.Format("{0} product(s) were added, and {1} products were updated.", newProducts, updatedProducts));
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem importing the product list: " + ex.Message);
            }

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult RelatedProducts(HttpPostedFileBase crossRefFile)
        {
            try
            {
                if (crossRefFile == null || crossRefFile.ContentLength <= 0 || Path.GetExtension(crossRefFile.FileName).ToLower() != ".txt")
                    throw new Exception("You must provide a valid tab delimited txt file.");

                string importPath = Server.MapPath("~/Public/Imports");
                if (!Directory.Exists(importPath))
                {
                    var di = Directory.CreateDirectory(importPath);
                    // TODO: permissions?
                }

                var storedFilePath = String.Format("{0}\\{1}", importPath, Path.GetFileName(crossRefFile.FileName));
                crossRefFile.SaveAs(storedFilePath);

                var engine = new FileHelperEngine(typeof(ImportedCrossReference));
                ImportedCrossReference[] importedCrossReferences = engine.ReadFile(storedFilePath) as ImportedCrossReference[];

                int newReferences = 0;
                
                // validate all the records so we can bail from the whole thing if something is wrong.
                var invalidRecords = new List<ImportedCrossReference>();
                foreach (var item in importedCrossReferences)
                {
                    var parentProduct = _ProductRepository.All.FirstOrDefault(x => x.ItemNumber == item.ParentItemNumber);
                    if (parentProduct == null)
                        invalidRecords.Add(item);
                        //throw new Exception("A product with item number <em>" + item.ParentItemNumber + "</em> could not be found.");

                    var childProduct = _ProductRepository.All.FirstOrDefault(x => x.ItemNumber == item.ChildItemNumber);
                    if (childProduct == null)
                        invalidRecords.Add(item);
                        //throw new Exception("A product with item number <em>" + item.ChildItemNumber + "</em> could not be found.");
                }

                
                foreach (var item in importedCrossReferences)
                {
                    // if the item is invalid, skip it
                    if (invalidRecords.Contains(item))
                        continue;

                    var parentProduct = _ProductRepository.All.FirstOrDefault(x => x.ItemNumber == item.ParentItemNumber);
                    
                    var childProduct = _ProductRepository.All.FirstOrDefault(x => x.ItemNumber == item.ChildItemNumber);
                    if (parentProduct != null)
                    {
                        switch (item.ReferenceTypeCode)
                        {
                            case 1: // other sizes
                                //var newSize = new RelatedProductSize { Product = childProduct };
                                //parentProduct.RelatedSizes.Add(newSize);
                                parentProduct.RelatedSizes.Add(childProduct);
                                _ProductRepository.InsertOrUpdate(parentProduct);
                                _ProductRepository.Save();
                                newReferences++;
                                break;
                            case 2: // accents
                                parentProduct.RelatedAccents.Add(childProduct);
                                _ProductRepository.InsertOrUpdate(parentProduct);
                                _ProductRepository.Save();
                                newReferences++;
                                break;
                            case 3: // trims
                                parentProduct.RelatedTrims.Add(childProduct);
                                _ProductRepository.InsertOrUpdate(parentProduct);
                                _ProductRepository.Save();
                                newReferences++;
                                break;
                            case 4: // finishes
                                parentProduct.RelatedFinishes.Add(childProduct);
                                _ProductRepository.InsertOrUpdate(parentProduct);
                                _ProductRepository.Save();
                                newReferences++;
                                break;
                        }
                    }
                }

                System.IO.File.Delete(storedFilePath);

                if (invalidRecords.Any())
                {
                    StringBuilder sb = new StringBuilder();
                    invalidRecords.ForEach(x =>
                    {
                        sb.AppendLine(String.Format("{0} - {1} - {2}", x.ParentItemNumber, x.ReferenceTypeCode, x.ChildItemNumber));
                    });
                    this.FlashError(String.Format("{0} product references were added. The following product references could not be matched in the database: {1}", newReferences, sb.ToString()));
                }
                else
                {
                    this.FlashInfo(String.Format("{0} product references were added.", newReferences));
                }
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem importing the product cross-references: " + ex.Message);
            }

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProductPriceCodes(HttpPostedFileBase priceCodeFile)
        {
            try
            {
                if (priceCodeFile == null || priceCodeFile.ContentLength <= 0 || Path.GetExtension(priceCodeFile.FileName).ToLower() != ".txt")
                    throw new Exception("You must provide a valid tab delimited txt file.");

                string importPath = Server.MapPath("~/Public/Imports");
                if (!Directory.Exists(importPath))
                {
                    var di = Directory.CreateDirectory(importPath);
                    // TODO: permissions?
                }

                var storedFilePath = String.Format("{0}\\{1}", importPath, Path.GetFileName(priceCodeFile.FileName));
                priceCodeFile.SaveAs(storedFilePath);

                var engine = new FileHelperEngine(typeof(ImportedProductPriceCode));
                ImportedProductPriceCode[] importedPriceCodes = engine.ReadFile(storedFilePath) as ImportedProductPriceCode[];

                int newCodes = 0;
                int updatedCodes = 0;

                foreach (var item in importedPriceCodes)
                {
                    if (_ProductPriceCodeRepo.All.Any(x => x.GroupName == item.PriceGroup && x.PriceCode == item.PriceCode))
                    {
                        var priceCode = _ProductPriceCodeRepo.All.FirstOrDefault(x => x.GroupName == item.PriceGroup && x.PriceCode == item.PriceCode);
                        priceCode.Price = item.Price;
                        _ProductPriceCodeRepo.InsertOrUpdate(priceCode);
                        updatedCodes++;
                    }
                    else
                    {
                        var priceCode = new ProductPriceCode();
                        priceCode.GroupName = item.PriceGroup;
                        priceCode.PriceCode = item.PriceCode;
                        priceCode.Price = item.Price;
                        _ProductPriceCodeRepo.InsertOrUpdate(priceCode);
                        newCodes++;
                    }
                    
                }

                _ProductPriceCodeRepo.Save();

                System.IO.File.Delete(storedFilePath);
                this.FlashInfo(String.Format("{0} product price codes were added, and {1} product price codes were updated.", newCodes, updatedCodes));
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem importing the product price codes: " + ex.Message);
            }

            return RedirectToAction("index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProductSeries(HttpPostedFileBase seriesFile)
        {
            try
            {
                if (seriesFile == null || seriesFile.ContentLength <= 0 || Path.GetExtension(seriesFile.FileName).ToLower() != ".txt")
                    throw new Exception("You must provide a valid tab delimited txt file.");

                string importPath = Server.MapPath("~/Public/Imports");
                if (!Directory.Exists(importPath))
                {
                    var di = Directory.CreateDirectory(importPath);
                    // TODO: permissions?
                }

                var storedFilePath = String.Format("{0}\\{1}", importPath, Path.GetFileName(seriesFile.FileName));
                seriesFile.SaveAs(storedFilePath);

                var engine = new FileHelperEngine(typeof(ImportedProductSeries));
                ImportedProductSeries[] imported = engine.ReadFile(storedFilePath) as ImportedProductSeries[];

                int newCodes = 0;
                int updatedCodes = 0;
                
                foreach (var item in imported)
                {
                    if (_ProductSeriesRepo.All.Any(x => x.Code == item.Code))
                    {
                        var series = _ProductSeriesRepo.All.FirstOrDefault(x => x.Code == item.Code);
                        series.Code = item.Code;
                        series.Name = item.Name;
                        string codeString = series.Code.ToString();
                        if (_ProductRepository.All.Any(x => (x.Series != null && x.Series != "") && x.Series == codeString))
                        {
                            foreach (var prod in _ProductRepository.All.Where(x => x.Series == codeString))
                            {
                                prod.ProductSeries = series;
                            }
                        }

                        _ProductSeriesRepo.InsertOrUpdate(series);
                        updatedCodes++;
                    }
                    else
                    {
                        var series = new ProductSeries();
                        series.Name = item.Name;
                        series.Code = item.Code;
                        string codeString = series.Code.ToString();
                        if (_ProductRepository.All.Any(x => (x.Series != null && x.Series != "") && x.Series == codeString))
                        {
                            foreach (var prod in _ProductRepository.All.Where(x => x.Series == codeString))
                            {
                                prod.ProductSeries = series;
                            }
                        }
                        
                        _ProductSeriesRepo.InsertOrUpdate(series);
                        newCodes++;
                    }

                }

                _ProductSeriesRepo.Save();

                System.IO.File.Delete(storedFilePath);
                this.FlashInfo(String.Format("{0} product series were added, and {1} product series were updated.", newCodes, updatedCodes));
            }
            catch (Exception ex)
            {
                this.FlashError("There was a problem importing the product series: " + ex.Message);
            }

            return RedirectToAction("index");
        }

        private void ImportItemRecord(ImportedProduct item, Product product)
        {
            // value fields
            product.BreakingStrength = item.Breaking;
            product.CartonQuantity = item.CartonSize;
            product.CoefficientOfFrictionDry = item.CofDry;
            product.CoefficientOfFrictionWet = item.CofWet;
            product.Description = item.Description;
            product.IsChemicalResistant = item.Chemical ?? false;
            product.IsFrostResistant = item.Frost ?? false;
            product.ItemNumber = item.ItemNumber;
            product.LastModifiedBy = CurrentUser.UserName;
            product.LastModifiedOn = DateTime.Now;
            product.MadeIn = item.Origin;
            product.ManufacturerColor = item.MfgColor;
            product.ManufacturerItem = item.MfgItemNumber;
            product.RetailPrice = item.RPrice ?? 0;
            product.ScratchHardiness = item.Hardness;
            product.Series = item.Series;
            product.UnitsPerPiece = item.Upp;
            product.WaterAbsorption = item.Absorption;
            product.SizeDescription = item.Size;
            if (!String.IsNullOrWhiteSpace(item.StatusChangeDate))
            {
                DateTime statusChangeDate;
                if (DateTime.TryParse(item.StatusChangeDate, out statusChangeDate))
                    product.StatusChangedOn = statusChangeDate;
            }
            //product.PriceCodeGroup = item.PriceCodeGroup;
            //TODO: Need to rewire the import to accommodate the new PriceGroup entity.

            // relationships
            int seriesCode = 0;
            if (Int32.TryParse(item.Series, out seriesCode))
            {
                product.Series = item.Series;
                var series = _ProductSeriesRepo.All.FirstOrDefault(x => x.Code == seriesCode);
                if (series != null)
                {
                    product.ProductSeries = series;
                }
                else
                {
                    series = new ProductSeries { Code = Int32.Parse(item.Series) };
                    _ProductSeriesRepo.InsertOrUpdate(series);
                }
            }
            
            if (item.Category.HasValue && item.Category.Value > 0)
                product.Category = _ProductcategoryRepository.All.FirstOrDefault(x => x.Code == item.Category.Value);
            
            if (item.KeyColor.HasValue)
                product.Color = _ProductcolorRepository.All.FirstOrDefault(x => x.Code == item.KeyColor.Value);
            
            
            if (item.Finish.HasValue && item.Finish.Value > 0)
                product.Finish = _ProductfinishRepository.All.FirstOrDefault(x => x.Code == item.Finish.Value);
            
            if (item.ProductGroup.HasValue && item.ProductGroup.Value > 0)
                product.Group = _ProductgroupRepository.All.FirstOrDefault(x => x.Code == item.ProductGroup.Value);

            if (!String.IsNullOrWhiteSpace(item.Mfg))
            {
                product.Section = item.Mfg;
                Manufacturer mfg = _ManufacturerRepository.All.FirstOrDefault(x => x.Section == item.Mfg);
                if (mfg == null)
                {
                    mfg = new Manufacturer { Section = item.Mfg };
                    _ManufacturerRepository.InsertOrUpdate(mfg);
                }
                product.Manufacturer = mfg;
            }

            if (item.SizeGroup.HasValue && item.SizeGroup.Value > 0)
                product.Size = _ProductsizeRepository.All.FirstOrDefault(x => x.Code == item.SizeGroup.Value);
            if (!String.IsNullOrWhiteSpace(item.StatusCode))
                product.Status = _ProductStatusRepository.All.FirstOrDefault(x => x.Code == item.StatusCode);
            if (item.Type.HasValue && item.Type.Value > 0)
                product.Type = _ProducttypeRepository.All.FirstOrDefault(x => x.Code == item.Type.Value);
            if (!String.IsNullOrWhiteSpace(item.UoM))
                product.UnitOfMeasure = _ProductUomRepository.All.FirstOrDefault(x => x.Code == item.UoM);
            if (item.UseCode.HasValue && item.UseCode.Value > 0)
                product.Usage = _ProductUsageRepository.All.FirstOrDefault(x => x.Code == item.UseCode.Value);
            if (item.Variation.HasValue && item.Variation.Value > 0)
                product.Variation = _ProductVariationRepository.All.FirstOrDefault(x => x.Code == item.Variation.Value);
        }
    }
}

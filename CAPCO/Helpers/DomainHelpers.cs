using System;
using CAPCO.Infrastructure.Domain;
using System.Web.Hosting;
using System.IO;
using System.Collections.Generic;
using CAPCO.Infrastructure.Data;
using System.Linq;
using System.Configuration;
using CAPCO.Infrastructure.Services;

namespace System.Web.Mvc
{
    public enum ImageSize
    {
    	Detail,
        Large,
        Small,
        Thumb
    }

    public static class DomainHelpers
    {
        const string pubDir = "Public";
        const string productImagesDir = "ProductImages";
        static string productImagesPath = ConfigurationManager.AppSettings["ProductImagePath"];

        private static string GetPublicUrl(this HtmlHelper helper)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            string appUrl = urlHelper.Content("~/");
            return appUrl + pubDir;
        }

        public static bool ImageExists(this UrlHelper url, Product product, ImageSize size)
        {
            var fileName = GetProductImageFileName(product, size) + ".jpg";
            var imageUrl = url.Content(productImagesPath + fileName);
            return File.Exists(HttpContext.Current.Server.MapPath(imageUrl));
        }

        //public static MvcHtmlString GetProductImageUrl(this UrlHelper url, Product product, ImageSize size)
        //{
        //    var fileName = GetProductImageFileName(product, size) + ".jpg";
            
        //    var pubUrl = String.Format("../{0}/{1}/",  pubDir, productImagesDir);
        //    var imageUrl = pubUrl + fileName;
            
        //    var imageExists = File.Exists(imageUrl);

        //    if (imageExists)
        //    {
        //        return MvcHtmlString.Create(String.Format("{0}", imageUrl));
        //    }
        //    else
        //    {
        //        return MvcHtmlString.Create("../Public/Images/noProductThumb.gif");
        //    }
        //}

        public static MvcHtmlString GetProductImage(this HtmlHelper helper, Product product)
        {
            return GetProductImage(helper, product, ImageSize.Large);
        }

        public static MvcHtmlString GetProductImage(this HtmlHelper helper, Product product, ImageSize size)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            var fileName = GetProductImageFileName(product, size) + ".jpg";

            var imageUrl = url.Content(productImagesPath + fileName);

            var imageExists = File.Exists(HttpContext.Current.Server.MapPath(imageUrl));

            // if yes, then build the url and return it
            if (imageExists)
            {
                return MvcHtmlString.Create(String.Format("<img src=\"{0}\" style=\"display: block;\" />", imageUrl));
            }
            else
            {
                return helper.Image("noProductThumb.gif", "style=display:block;");
            }
        }

        private static string GetProductImageFileName(Product product, ImageSize size)
        {
            if (product != null)
            {
                switch (size)
                {
                    case ImageSize.Detail:
                        return product.DetailImageFileName;
                    case ImageSize.Large:
                        return product.LargeImageFileName;
                    case ImageSize.Small:
                        return product.SmallImageFileName;
                    case ImageSize.Thumb:
                        return product.ThumbnailImageFileName;
                    default:
                        return product.LargeImageFileName;
                }
            }
            else
                return string.Empty;
        }

        public static List<ProductPriceCode> PriceCodes(this Product product)
        {
            var repo = DependencyResolver.Current.GetService<IProductPriceCodeRepository>();

            if (repo.All.Any(x => x.PriceGroup == product.PriceCodeGroup))
            {
                return repo.All.Where(x => x.PriceGroup == product.PriceCodeGroup).ToList();
            }

            return new List<ProductPriceCode>();
        }
    }
}

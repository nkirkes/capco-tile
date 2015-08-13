using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileHelpers;

namespace CAPCO.Areas.Admin.Models
{
    [DelimitedRecord("\t"), IgnoreEmptyLines]
    public class ImportedProductPriceCode
    {
        public string PriceGroup { get; set; }
        public string PriceCode { get; set; }
        public decimal Price { get; set; }
    }

    [DelimitedRecord("\t"), IgnoreEmptyLines]
    public class ImportedCrossReference
    {
        public string ParentItemNumber { get; set; }
        public int ReferenceTypeCode { get; set; }
        public string ChildItemNumber { get; set; }
    }

    [DelimitedRecord("\t"), IgnoreEmptyLines]
    public class ImportedProductSeries
    {
        public string Name;
        public int Code;
        public string Comment;
        [FieldConverter(ConverterKind.Boolean, "TRUE", "FALSE")]
        public bool? InPriceList;
    }

    [DelimitedRecord("\t"), IgnoreEmptyLines]
    public class ImportedCustomer
    {
        public string UserName;
        public string Password;
        public string FirstName;
        public string LastName;
        public string CompanyName;
        public string DefaultLocationId;
        public string DefaultDiscountCode;
        public string AccountNumber;
        public string StreetAddressLine1;
        public string StreetAddressLine2;
        public string City;
        public string State;
        public string ZipCode;
        public string Email;
        public string Phone;
        public string Fax;
        public string DefaultPricePreference;
        [FieldConverter(ConverterKind.Boolean, "Y", "N")]
        public bool OptInMarketing;
        [FieldConverter(ConverterKind.Boolean, "Y", "N")]
        public bool OptInSystem;
    }

    // Property order is important! It maps to the incoming CSV format.
    [DelimitedRecord("\t"), IgnoreEmptyLines]
    public class ImportedProduct
    {
        public string ItemNumber;
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        public string Description;
        public string Mfg;
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        public string Size;
        public string Series;
        public string MfgColor;
        [FieldQuoted(QuoteMode.OptionalForBoth)]
        public string MfgItemNumber;
        public string StatusCode;
        public int? UseCode;
        public bool? Frost;
        public int? Variation;
        public string Origin;
        public string UoM;
        public decimal? Upp;
        public decimal? CartonSize;
        public string Absorption;
        public string Breaking;
        [FieldConverter(ConverterKind.Boolean, "TRUE", "FALSE")]
        public bool? Chemical;
        public string Hardness;
        public decimal? CofWet;
        public decimal? CofDry;
        public decimal? RPrice;
        public int? ProductGroup;
        public int? Category;
        public int? Type;
        public int? KeyColor;
        public int? SizeGroup;
        public int? Finish;
        public string ChangeDate;
        public string StatusChange;
        public string StatusChangeDate;
        public string PriceCodeGroup;
        public decimal? Dcof;
    }
}
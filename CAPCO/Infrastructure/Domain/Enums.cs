using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace CAPCO.Infrastructure.Domain
{
    public enum AccountStatus
    {
        Active,
        Flagged,
        Suspended
    }

    public enum PriceLists
    {
        All,
        Section,
        Updates,
        None
    }

    public enum UserRoles
    {
        Administrators,
        [Description("Application Users")]
        ApplicationUsers,
        Consumers,
        [Description("Service Providers")]
        ServiceProviders
    }

    public enum PricePreferences
    {
        [Description("Retail Only")]
        RetailOnly,
        [Description("Cost Only")]
        CostOnly,
        Both,
        None
    }

    public enum AccountTypes
    {
        Consumer,
        [Description("Service Provider")]
        ServiceProvider
    }

    public enum States
    {
        Alabama,
        Alaska,
        Arizona,
        Arkansas,
        California,
        Colorado,
        Connecticut,
        Delaware,
        Florida,
        Georgia,
        Hawaii,
        Idaho,
        Illinois,
        Indiana,
        Iowa,
        Kansas,
        Kentucky,
        Louisiana,
        Maine,
        Maryland,
        Massachusetts,
        Michigan,
        Minnesota,
        Mississippi,
        Missouri,
        Montana,
        Nebraska,
        Nevada,
        [Description("New Hampshire")]
        NewHampshire,
        [Description("New Jersey")]
        NewJersey,
        [Description("New Mexico")]
        NewMexico,
        [Description("New York")]
        NewYork,
        [Description("North Carolina")]
        NorthCarolina,
        [Description("North Dakota")]
        NorthDakota,
        Ohio,
        Oklahoma,
        Oregon,
        Pennsylvania,
        [Description("Rhode Island")]
        RhodeIsland,
        [Description("South Carolina")]
        SouthCarolina,
        [Description("South Dakota")]
        SouthDakota,
        Tennessee,
        Texas,
        Utah,
        Vermont,
        Virginia,
        Washington,
        [Description("West Virginia")]
        WestVirginia,
        Wisconsin,
        Wyoming
    }
}
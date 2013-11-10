/* common ui functions */
var capco = {
    /* common configuration */
    config: {},
    /* common utility functions */
    utils: {},
    /* common ui functions */
    ui: {}
};


capco.utils = {
    isUndefined: function (item) {
        return typeof (item) === "undefined";
    }
};

capco.config = {
    dataTable: {
        paginate: true,
        searchable: true,
        showInfo: true,
        selectLength: false,
        rows: 25
    }
};

capco.ui = {
    /** renders an html as a sortable table via jquery plugin */
    toDataTable: function (selector, columnSortingConfig, config) {
        var table = $(selector);
        utils = capco.utils;
        config = config || capco.config.dataTable;
        if (table.find('td').length > 1) {
            //initialize the grid
            table.addClass("dataTable").dataTable({
                bFilter: utils.isUndefined(config.searchable) ? capco.config.dataTable.searchable : config.searchable,
                bPaginate: utils.isUndefined(config.paginate) ? capco.config.dataTable.paginate : config.paginate,
                bInfo: utils.isUndefined(config.showInfo) ? capco.config.dataTable.showInfo : config.showInfo,
                bLengthChange: utils.isUndefined(config.selectLength) ? capco.config.selectLength : config.selectLength,
                bAutoWidth: utils.isUndefined(config.bAutoWidth) ? capco.config.dataTable.bAutoWidth : config.bAutoWidth,
                bProcessing: utils.isUndefined(config.bProcessing) ? false : config.bProcessing,
                bServerSide: utils.isUndefined(config.bServerSide) ? false : config.bServerSide,
                sAjaxSource: utils.isUndefined(config.sAjaxSource) ? null : config.sAjaxSource,
                bSortClasses: false,
                iDisplayLength: capco.config.dataTable.rows,

                aoColumns: columnSortingConfig
            }).css("visibility", "visible"); //to prevent flash of grid before it is dataTabled it is initially rendered as visibility:hidden after it is dataTabled we will show it
        } else {
            table.css("visibility", "visible")
                .addClass('dataTables_wrapper')
                .find("thead").hide();
        }
    }
};
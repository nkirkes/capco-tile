$.fn.equalHeight = function () {
    return this.height(Math.max.apply(this, $.map(this, function (e) { return $(e).height() })));
}

$.fn.setHeight = function (height) {

    $(this).each(function () {
        
        $(this).css({ 'height': height });
    });
    return this;
};
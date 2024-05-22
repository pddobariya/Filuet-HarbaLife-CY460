$(document).ready(function () {

    $('#checkout-steps .tab-section:visible').each(function (i, s) {
        $('.step-title .number', this).html((i + 1) + '');
    });
})


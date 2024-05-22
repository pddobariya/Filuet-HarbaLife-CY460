FiluetShopingService = {
    ShowMessage: function (isCartValid, skus) {
        $(function () {
            var dialogIsCartValid = $("#dialog-iscartvalid");
            dialogIsCartValid.dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                buttons: [{
                    text: dialogIsCartValid.attr("data-close_button"),
                    click: function () {
                        $(this).dialog("close");
                    }
                }]
            }).dialog("close");
            if (typeof isCartValid === "string" && isCartValid.toLowerCase() === "false" || isCartValid === false) {
                var dialogIsCartValidSpanElement = dialogIsCartValid.find("span");
                dialogIsCartValidSpanElement.html(dialogIsCartValid.attr("data-template").replace("{0}", skus));
                dialogIsCartValid.dialog("open");
                $("body>.ui-widget-overlay").css({position: "fixed", top: 0, left: 0, width: "100%", height: "100%"});
                dialogIsCartValid.attr("data-cartIsValid", false);
            } else {
                dialogIsCartValid.attr("data-cartIsValid", true);
            }
            this.DisableCheckout();
        });
    },
    SetDualMonth: function (isSelected) {
        var dialogIsCartValid = $("#dialog-iscartvalid");
        if (isSelected) {
            dialogIsCartValid.attr("data-monthIsSelected", true);
        } else {
            dialogIsCartValid.attr("data-monthIsSelected", false);
        }
        this.DisableCheckout();
    },
    DisableCheckout: function () {
        var dialogIsCartValid = $("#dialog-iscartvalid");
        var cartIsValid = dialogIsCartValid.attr("data-cartIsValid").toLowerCase() === "true";
        var monthIsSelected = dialogIsCartValid.attr("data-monthIsSelected").toLowerCase() === "true";
        if (cartIsValid && monthIsSelected) {
            $('.checkout-buttons>button').prop('disabled', false).css('opacity', 1);
        } else {
            $('.checkout-buttons>button').prop('disabled', true).css('opacity', 0.5);
        }
    }
}


﻿!(function (u, c, l, s) {
    var r = ".ajaxCartInfo";
    function i() {
        return l.getAttrValFromDom(r, "data-productBoxProductItemElementSelector");
    }
    function p(t) {
        return l.getAttrValFromDom(t, "value") || t.text();
    }
    function a() {
        var t = l.getAttrValFromDom(r, "data-productPageAddToCartButtonSelector"),
            a = l.getAttrValFromDom(r, "data-productBoxAddToCartButtonSelector"),
            n = [],
            d = [];
        return (
            c(t).each(function () {
                if (!c(this).is(".nopAjaxCartProductVariantAddToCartButton")) {
                    var t = l.getAttrValFromDom(this, "data-productid", 0);
                    if (t && 0 < t) {
                        var a = { productId: t, isProductPage: !0, buttonElement: c(this) },
                            e = { productId: t, isProductPage: !0, buttonValue: p(c(this)) };
                        n.push(a), d.push(e);
                    }
                }
            }),
            c(a).each(function () {
                if (!c(this).is(".nopAjaxCartProductListAddToCartButton")) {
                    var t = ((o = c(this)), (r = i()) == s || "" === r ? 0 : l.getAttrValFromDom(c(o).parents(r), "data-productid", 0));
                    if (t && 0 < t) {
                        var a = { productId: t, isProductPage: !1, buttonElement: c(this) },
                            e = { productId: t, isProductPage: !1, buttonValue: p(c(this)) };
                        n.push(a), d.push(e);
                    }
                }
                var o, r;
            }),
            { addToCartButtons: n, buttonsParameters: d }
        );
    }
    function n() {
        var t = c("body");
        0 < t.length && c(".nopAjaxCartPanelAjaxBusy").height(t.height()).width(t.width()).show();
    }
    function e(t) {
        t
            ? (function () {
                if (0 < c(".miniProductDetailsView").length) {
                    var t = c(".miniProductDetailsView").parent().height(),
                        a = c(".miniProductDetailsView").parent().width();
                    c(".miniProductDetailsView .miniProductDetailsPanelAjaxBusy")
                        .height(t - 10)
                        .width(a - 10)
                        .show();
                }
            })()
            : n();
    }
    function g() {
        c(".nopAjaxCartPanelAjaxBusy").hide();
    }
    function m(t) {
        t ? c(".miniProductDetailsView .miniProductDetailsPanelAjaxBusy").hide() : g();
    }
    function o(t, a) {
        var e = c(t).filter("div[data-productId='" + a.productId + "']");
        if (0 < e.length) {
            var o = l.getAttrValFromDom(e, "data-isProductPage");
            o && o === a.isProductPage.toString() && (c(a.buttonElement).parents(".product-item, .add-to-cart").addClass("sevenspikes-ajaxcart"), c(a.buttonElement).replaceWith(e[0]));
        }
    }
    function d() {
        var e = a(),
            t = e.buttonsParameters;
        0 < t.length &&
            c.ajax({ cache: !1, type: "POST", data: c.toJSON(t), contentType: "application/json; charset=utf-8", url: l.getAttrValFromDom(r, "data-getAjaxCartButtonUrl") }).done(function (t) {
                if ("" !== t) {
                    for (var a = 0; a < e.addToCartButtons.length; a++) o(t, e.addToCartButtons[a]);
                    c.event.trigger({ type: "nopAjaxCartButtonsAddedEvent" });
                }
            });
    }
    function h() {
        var e = ".block.block-shoppingcart";
        0 < c(e).length &&
            c.ajax({ cache: !1, type: "GET", url: l.getHiddenValFromDom("#miniShoppingCartUrl") }).done(function (t) {
                var a = c(t).filter(e);
                c(e).replaceWith(a), c.event.trigger({ type: "nopAjaxCartMiniShoppingCartUpdated" });
            });
    }
    function v(t) {
        t = '<div class="miniProductDetailsPanelAjaxBusy"></div><div class="clear"></div>' + t;
        var a = c(".miniProductDetailsView").data("kendoWindow").content(t);
        a.center(),
            a.open(),
            c(document).on("ajaxCart.product_attributes_changed", function (t) {
                if (t.changedData && t.changedData.pictureDefaultSizeUrl) {
                    var a = t.changedData.pictureDefaultSizeUrl;
                    t.element.closest(".product-overview-line").find(".picture img").attr("src", a);
                }
            });
    }
    function f(t, a, e) {
        var o = { productId: t, isAddToCartButton: e },
            r = l.getHiddenValFromDom("#getMiniProductDetailsViewUrl");
        c.ajax({ cache: !1, async: !1, type: "POST", data: o, url: r }).done(function (t) {
            v(t),
                (function (t) {
                    var a = l.getAttrValFromDom(".miniProductDetailsView .miniProductDetailsViewAddToCartButton, .miniProductDetailsView .nopAjaxCartProductVariantAddToWishlistButton", "data-productId", 0);
                    if (0 < a) {
                        var e = c(".miniProductDetailsView #addtocart_" + a + "_EnteredQuantity");
                        e.is("select") && (e.find("option[value=" + t + "]").length || (t = e.find("option:first").val())), e.val(t);
                    }
                })(a),
                c.event.trigger({ type: "nopAjaxCartMiniProductDetailsViewShown" });
        });
    }
    function C() {
        var e = l.getHiddenValFromDom("#flyoutShoppingCartPanelSelector");
        u.shouldRefreshFlyoutCart &&
            0 < c(e).length &&
            c.ajax({ cache: !1, type: "GET", url: l.getHiddenValFromDom("#flyoutShoppingCartUrl") }).done(function (t) {
                var a = c(t).filter(e);
                c(e).replaceWith(a), c.event.trigger({ type: "nopAjaxCartFlyoutShoppingCartUpdated" });
            });
    }
    function A() {
        var e = l.getHiddenValFromDom("#shoppingCartBoxPanelSelector");
        0 < c(e).length &&
            c.ajax({ cache: !1, type: "GET", url: l.getHiddenValFromDom("#shoppingCartBoxUrl") }).done(function (t) {
                var a = c(t).find(e);
                c(e).replaceWith(a), c.event.trigger({ type: "nopAjaxCartShoppingCartBoxUpdated" });
            });
    }
    function w(d) {
        var t = '.header-links a[href="/cart"]',
            a = l.getHiddenValFromDom("#shoppingCartMenuLinkSelector");
        0 < c("#shoppingCartMenuLinkSelector").length && "" !== a && ((t = a), (t = c("<div/>").html(t).text())),
            c(t).each(function (t, a) {
                var e = c(a).html(),
                    o = /\d+/.exec(e),
                    r = parseInt(o) + parseInt(d),
                    n = c(".ajaxCartInfo").attr("data-miniShoppingCartQuatityFormattingResource").replace("{0}", r);
                c(a).html(n);
            });
    }
    function P(d) {
        var t = "span.wishlist-qty",
            a = l.getHiddenValFromDom("#wishlistMenuLinkSelector");
        0 < c(a).length && (t = a),
            c(t).each(function (t, a) {
                var e = c(a).html(),
                    o = /\d+/.exec(e),
                    r = parseInt(o) + parseInt(d),
                    n = c(".ajaxCartInfo").attr("data-miniWishlistQuatityFormattingResource").replace("{0}", r);
                c(a).html(n);
            });
    }
    function x(t) {
        c(".productAddedToCartWindow").html(c(t).html());
        var a = c(".productAddedToCartWindow").data("kendoWindow");
        a.center(), a.open();
    }
    function T(t, a) {
        var e = c(".addProductToCartErrors").data("kendoWindow");
        e ||
            (e = c(".addProductToCartErrors")
                .kendoWindow({ draggable: !1, resizable: !1, width: "300px", height: "100px", modal: !0, actions: ["Close"], animation: !1, visible: !1, title: a })
                .data("kendoWindow")).wrapper.addClass("ajaxCart"),
            e.content(t),
            e.center(),
            e.open(),
            c(document).on("click", ".k-overlay", function () {
                e.close();
            });
    }
    function DialogToBiz(t, a) {
        var el = c(".addProductToCartErrors")
            .kendoDialog({
                draggable: false,
                resizable: false,
                width: "500px",
                //height: "100px",
                modal: true,
                actions: [
                    {
                        text: "ДА",
                        action: function(e) {
                            el.close();
                            if (!u.waitForAjaxRequest) {
                                n();
                                u.waitForAjaxRequest = true;
                            }
                            $.ajax({
                                method: "POST",
                                //data: { mainUrl: document.location.pathname },
                                url: l.getHiddenValFromDom("#addBizWorkToCart"),
                                async: true,
                                success: function(data, status, xhr) {
                                    document.location = l.getHiddenValFromDom("#shoppingCart");
                                    return true;
                                },
                                error: function(xhr, status, error) {
                                    alert(error);
                                }
                            }).always(function() {
                                g(), (u.waitForAjaxRequest = false);
                            });
                            return false;
                        }
                    }, {
                        text: "НЕТ"
                    }
                ],
                content: t,
                animation: false,
                visible: false,
                title: a,
                closable: false
            })
            .data("kendoDialog").wrapper.addClass("ajaxCart");
            el.center(),
            el.open(),
            c(document).on("click", ".k-overlay", function () {
                el.close();
            });
    }
    function DialogFromBiz(t, a, d, callBack, clearAndAddFunc) {
        var el = c(".addProductToCartErrors")
            .kendoDialog({
                draggable: false,
                resizable: false,
                width: "500px",
                //height: "100px",
                modal: true,
                actions: [
                    {
                        text: "ДА",
                        action: function(e) {
                            el.close();
                            if (!u.waitForAjaxRequest) {
                                n();
                                u.waitForAjaxRequest = true;
                            }
                            c.ajax({ cache: false, type: "POST", data: d, url: l.getHiddenValFromDom(clearAndAddFunc) })
                                .done(function(respone) {
                                    callBack(respone, callBack);
                                })
                                .always(function() {
                                    g(), (u.waitForAjaxRequest = !1);
                                });
                            return false;
                        }
                    }, {
                        text: "Оплатить bizwork",
                        action: function(e) {
                            document.location = l.getHiddenValFromDom("#shoppingCart");
                        }
                    }
                ],
                content: t,
                animation: false,
                visible: false,
                title: a,
                closable: false
            })
            .data("kendoDialog").wrapper.addClass("ajaxCart");
            el.center(),
            el.open(),
            c(document).on("click", ".k-overlay", function () {
                el.close();
            });
    }
    function ClearsShoppingCartMenuLinkSelector() {
        var t = '.header-links a[href="/cart"]',
            a = l.getHiddenValFromDom("#shoppingCartMenuLinkSelector");
        0 < c("#shoppingCartMenuLinkSelector").length && "" !== a && ((t = a), (t = c("<div/>").html(t).text())),
            c(t).each(function (t, a) {
                var r = '0',
                    n = c(".ajaxCartInfo").attr("data-miniShoppingCartQuatityFormattingResource").replace("{0}", r);
                c(a).html(n);
            });
    }
    function DialogClearAndAdd(t, a, d, callBack, clearAndAddFunc) {
        var el = c(".addProductToCartErrors")
            .kendoDialog({
                draggable: false,
                resizable: false,
                width: "500px",
                //height: "100px",
                modal: true,
                actions: [
                    {
                        text: "ДА",
                        action: function(e) {
                            el.close();
                            if (!u.waitForAjaxRequest) {
                                n();
                                u.waitForAjaxRequest = true;
                            }
                            c.ajax({ cache: false, type: "POST", data: d, url: l.getHiddenValFromDom(clearAndAddFunc) })
                                .done(function (respone) {
                                    ClearsShoppingCartMenuLinkSelector();
                                    callBack(respone, callBack);
                                })
                                .always(function() {
                                    g(), (u.waitForAjaxRequest = !1);
                                });
                            return false;
                        }
                    }, {
                        text: "Нет"
                    }
                ],
                content: t,
                animation: false,
                visible: false,
                title: a,
                closable: false
            })
            .data("kendoDialog");
            el.wrapper.addClass("ajaxCart"),
            el.center(),
            el.open(),
            c(document).on("click", ".k-overlay", function () {
                el.close();
            });
    }
    function j(t, a, e, o, r, n) {
        var d = { productId: o, quantity: r, isAddToCartButton: e };
        var AddProductDone = function (t, callBack) {
            "success" === t.Status
                ? e
                    ? (h(), C(), A(), w(r), x(t.ProductAddedToCartWindow), c.event.trigger({ type: "nopAjaxCartProductAddedToCartEvent", productId: o, quantity: r }))
                    : (P(r), x(t.ProductAddedToCartWindow), c.event.trigger({ type: "nopAjaxCartProductAddedToWishlistEvent", productId: o, quantity: r }))
                : "warning" === t.Status
                    ? T(t.AddToCartWarnings, t.PopupTitle)
                    : "bizwork" === t.Status
                        ? DialogToBiz(t.AddToCartWarnings, t.PopupTitle)
                        : "frombizwork" === t.Status
                            ? DialogFromBiz(t.AddToCartWarnings, t.PopupTitle, d, callBack, "#clearCartAndAddProductUrl")
                            : "productsconflict" === t.Status ? DialogClearAndAdd(t.AddToCartWarnings, t.PopupTitle, d, callBack, "#clearCartAndAddProductUrl") : "error" === t.Status && T(t.ErrorMessage, t.PopupTitle);
        };
        return n
            ? (g(), void (u.waitForAjaxRequest = !1))
            : t.HasProductAttributes
                ? (f(o, r, e), g(), void (u.waitForAjaxRequest = !1))
                : void c
                    .ajax({ cache: !1, type: "POST", data: d, url: l.getHiddenValFromDom("#addProductToCartUrl") })
                    .done(function (t) { AddProductDone(t, AddProductDone) })
                    .always(function () {
                        g(), (u.waitForAjaxRequest = !1);
                    });
    }
    function y(t, a) {
        if (!u.waitForAjaxRequest) {
            u.waitForAjaxRequest = !0;
            var e = l.getAttrValFromDom(t.currentTarget, "data-productId") || c(t.currentTarget).parents(".product-item").attr("data-productid"),
                o = (function (t) {
                    var a = 1,
                        e = l.getAttrValFromDom(t, "data-productId") || c(t).parents(".product-item").attr("data-productid") || "";
                    if ("" !== e) {
                        var o = c(t)
                            .parents(".product-item")
                            .find("[data-quantityproductid=" + e + "]");
                        if (0 < o.length) {
                            var r = o.val();
                            0 < r && (a = r);
                        }
                    }
                    return a;
                })(t.currentTarget),
                r = { productId: e, quantity: o };
            n(),
                c.ajax({ cache: !1, type: "POST", data: r, url: l.getHiddenValFromDom("#checkProductAttributesUrl") })
                    .done(function (t) {
                        j(t, 0, a, e, o, !1);
                    })
                    .fail(function () {
                        j(data, 0, a, e, o, !0);
                    });
        }
    }
    function D(o, r, n) {
        var AddProductDone = function(t, data, callBack) {
            if ("warning" === t.Status || "error" === t.Status) {
                var a = c(o.currentTarget).closest(".overview");
                !(function (t, a, e, o) {
                    if (e) 0 < o.length ? o.find(".message-error").html(t) : c(".miniProductDetailsView .message-error").html(t);
                    else {
                        var r = c(".addProductVariantToCartErrors").data("kendoWindow");
                        r ||
                            (r = c(".addProductVariantToCartErrors")
                                .kendoWindow({ draggable: !1, resizable: !1, width: "300px", modal: !0, actions: ["Close"], animation: !1, visible: !1, title: a })
                                .data("kendoWindow")).wrapper.addClass("ajaxCart"),
                            r.content(t),
                            r.center(),
                            r.open(),
                            c(document).on("click", ".k-overlay", function () {
                                r.close();
                            });
                    }
                })(t.AddToCartWarnings, t.PopupTitle, r, a);
            } else if ("bizwork" === t.Status) {
                DialogToBiz(t.AddToCartWarnings, t.PopupTitle);
            } else if ("frombizwork" === t.Status) {
                DialogFromBiz(t.AddToCartWarnings, t.PopupTitle, data, callBack, "#clearCartAndAddProductFromProductDetailsUrl");
            } else if ("productsconflict" === t.Status) {
                DialogClearAndAdd(t.AddToCartWarnings, t.PopupTitle, data, callBack, "#clearCartAndAddProductFromProductDetailsUrl");
            } else {
                var e = "nopAjaxCartProductAddedToCartEvent";
                n ? (h(), A(), w(i), C()) : (P(i), (e = "nopAjaxCartProductAddedToWishlistEvent")),
                    r && c(".miniProductDetailsView").data("kendoWindow").close(),
                    x(t.ProductAddedToCartWindow),
                    c.event.trigger({ type: e, productId: d, quantity: i });
            }
        }
        if ((r == s && (r = !1), !u.waitForAjaxRequest)) {
            u.waitForAjaxRequest = !0;
            var t = c(o.currentTarget),
                d = l.getAttrValFromDom(o.currentTarget, "data-productId") || c(o.currentTarget).parents(".product-item").attr("data-productid"),
                i = (function (t) {
                    var a = 1,
                        e = l.getAttrValFromDom(t, "data-productId");
                    if ("" !== e) {
                        var o = c("#addtocart_" + e + "_EnteredQuantity");
                        if (0 < o.length) {
                            var r = o.val();
                            0 < r && (a = r);
                        }
                    }
                    return a;
                })(t),
                a = t.closest("form").serialize();
            if (0 === a.length) {
                a = c("body").find("form").serialize();
            }
            0 !== (a = (a = a.replace(new RegExp("ajaxCart_product_attribute", "g"), "product_attribute")).replace(new RegExp("quickView_product_attribute", "g"), "product_attribute")).length && (a += "&"),
                (a += "productId=" + d),
                (a += "&isAddToCartButton=" + n),
                e(r),
                c.ajax({ cache: !1, type: "POST", data: a, url: l.getHiddenValFromDom("#addProductVariantToCartUrl") })
                .done(function (t) { AddProductDone(t, a, AddProductDone)})
                    .always(function () {
                        m(r), (u.waitForAjaxRequest = !1);
                    });
        }
    }
    (u.shouldRefreshFlyoutCart = !0),
        (u.waitForAjaxRequest = !1),
        (u.replaceAddToCartButtonsExternal = d),
        (u.closeProductAddedToCartWindow = function () {
            var t = c(".productAddedToCartWindow").data("kendoWindow");
            t && t.close();
        }),
        c(document).ready(function () {
            var t;
            d(),
                0 === (t = c("body")).find(".nopAjaxCartPanelAjaxBusy").length && (t.prepend('<div class="nopAjaxCartPanelAjaxBusy"></div>'), c(".nopAjaxCartPanelAjaxBusy").hide()),
                (function () {
                    var t = c("body");
                    if (0 < t.length) {
                        t.prepend('<div class="addProductToCartErrors"></div><div class="addProductVariantToCartErrors"></div><div class="miniProductDetailsView"></div><div class="productAddedToCartWindow"></div>');
                        var a = c(".miniProductDetailsView")
                            .kendoWindow({ draggable: !1, resizable: !1, modal: !0, actions: ["Close"], animation: !1, visible: !1 })
                            .data("kendoWindow");
                        a.wrapper.addClass("ajaxCart"),
                            c(document).on("click", ".k-overlay", function () {
                                a.close();
                            });
                    }
                    var e = c(".productAddedToCartWindow")
                        .kendoWindow({ draggable: !1, resizable: !1, modal: !0, actions: ["Close"], animation: !1, visible: !1 })
                        .data("kendoWindow");
                    e.wrapper.addClass("ajaxCart"),
                        c(document).on("click", ".k-overlay", function () {
                            e.close();
                        });
                })(),
                c("body").on("click", ".nopAjaxCartProductListAddToCartButton", function (t) {
                    t.preventDefault(), y(t, !0);
                }),
                c("body").on("click", ".nopAjaxCartProductVariantAddToCartButton", function (t) {
                    D(t, c(this).hasClass("miniProductDetailsViewAddToCartButton"), !0);
                }),
                c("body").on("click", ".miniProductDetailsViewAddToWishlistButton", function (t) {
                    D(t, !0, !1);
                }),
                c("body").on("click", ".productAddedToCartWindow .continueShoppingLink", function (t) {
                    t.preventDefault(), u.closeProductAddedToCartWindow();
                }),
                c(document).on("nopAjaxCartButtonsAddedEvent", function () {
                    var t, e, o, a;
                    (t = c(r)),
                        (e = "True" === t.attr("data-enableOnProductPage")),
                        (o = "True" === t.attr("data-enableOnCatalogPages")),
                        (a = t.attr("data-addToWishlistButtonSelector")),
                        c(a).each(function () {
                            var t = c(this),
                                a = i();
                            0 < t.parents(a).length
                                ? o &&
                                (t.prop("onclick", null).off("click"),
                                    t.on("click", function (t) {
                                        t.preventDefault(), y(t, !1);
                                    }))
                                : e &&
                                (t.prop("onclick", null).off("click"),
                                    t.on("click", function (t) {
                                        t.preventDefault(), D(t, !1, !1);
                                    }));
                        }),
                        (function () {
                            var t = c(".ajaxCartAllowedQuantitesHover");
                            if (0 !== t.length) {
                                var e = t.attr("data-productItemSelector");
                                c(".productQuantityDropdown")
                                    .off("mousedown")
                                    .on("mousedown", function () {
                                        var t = c(this).closest(e);
                                        t.addClass("ajax-cart-product-item-hover");
                                        var a = t.attr("data-productid");
                                        c(".productQuantityChanged" + a).val("yes");
                                    }),
                                    c(e)
                                        .off("mouseenter")
                                        .on("mouseenter", function () {
                                            var t = c(this).attr("data-productid"),
                                                a = c(".productQuantityChanged" + t);
                                            "yes" === a.val() && a.val("no");
                                        })
                                        .off("mouseleave")
                                        .on("mouseleave", function () {
                                            var t = c(this).attr("data-productid");
                                            "no" === c(".productQuantityChanged" + t).val() && c(this).removeClass("ajax-cart-product-item-hover");
                                        });
                            }
                        })();
                }),
                c(document).on("nopAjaxFiltersFiltrationCompleteEvent newProductsAddedToPageEvent", d),
                c(document).on("nopQuickViewDataShownEvent", d);
        });
})((window.nopAjaxCart = window.nopAjaxCart || {}), jQuery, sevenSpikesCore);

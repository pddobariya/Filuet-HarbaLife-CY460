
/**
 * Client-side JS Shopping Plugin API, constants and helper functions
 */
function ShoppingPlugin() { };

/*prototypes*/

String.prototype.format = function () {
    var s = this,
        i = arguments.length;

    while (i--) {
        s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
    }
    return s;
};

Date.prototype.getMonthName = function () {
    const d = this;
    return ShoppingPlugin.helpers.getMonthName(d.getMonth());
};

/*constants*/

ShoppingPlugin.constants = {};

ShoppingPlugin.constants.processingLocationChangeWarningMessage = 'Your selected delivery option differs from previously selected processing location. Resulting order total may differ from the one displayed in shopping cart.';
ShoppingPlugin.constants.cartTotalsFineprint = 'Total doesn\'t include possible shipping cost. Your total will be updated on the next page after you select shipping.';
ShoppingPlugin.constants.tokenUrlParam = 't';
ShoppingPlugin.constants.defaultShippingCalculationOptionId = 1;

/*helpers*/

ShoppingPlugin.helpers = {};

ShoppingPlugin.helpers.getMonthName = function (monthIndex) {
    const monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
        'July', 'August', 'September', 'October', 'November', 'December'
    ];
    return monthNames[monthIndex];
};

ShoppingPlugin.helpers.waitForElement = function (selector, callback, count) {
    if ($(selector).length && $(selector).is(':visible')) {
        callback();
    } else {
        setTimeout(function () {
            if (!count) {
                count = 0;
            }
            count++;
            if (count < 30) {
                ShoppingPlugin.helpers.waitForElement(selector, callback, count);
            }
            else {
                return;
            }
        }, 200);
    }
};

ShoppingPlugin.helpers.isNullOrWhiteSpace = function (input) {
    if (typeof input === 'undefined' || input == null) {
        return true;
    }
    return input.replace(/\s/g, '').length < 1;
};

ShoppingPlugin.helpers.getQueryParam = function (key) {
    key = key.replace(/[*+?^$.\[\]{}()|\\\/]/g, '\\$&'); // escape RegEx meta chars
    const match = location.search.match(new RegExp('[?&]' + key + '=([^&]+)(&|$)'));
    return match && decodeURIComponent(match[1].replace(/\+/g, ' ')).toLowerCase();
};

ShoppingPlugin.helpers.hasQueryParam = function (key) {
    const value = ShoppingPlugin.helpers.getQueryParam(key);
    return !ShoppingPlugin.helpers.isNullOrWhiteSpace(value);
};

ShoppingPlugin.helpers.getCountryCode = function (country) {
    if (country == null) {
        return null;
    }
    if (country.length == 2) {
        return country.toUpperCase();
    }
    switch (country.toLowerCase()) {
        case 'latvia':
            return 'LV';
        case 'lithuania':
            return 'LT';
        case 'estonia':
            return 'EE';
    };
    return null;
};

ShoppingPlugin.api = {};

ShoppingPlugin.api.call = function (url, data, onSuccess, onError) {
    if (data == null) {
        data = {};
    }

    //init session token from URL if it's present
    if (ShoppingPlugin.helpers.hasQueryParam(ShoppingPlugin.constants.tokenUrlParam)) {
        data.session_token = ShoppingPlugin.helpers.getQueryParam(ShoppingPlugin.constants.tokenUrlParam);
    }


    var onErrorCallback = function (data) {
        if (onError == null) {
            return;
        }
        if (data == null || data.error == null || data.error.msg == null) {
            onError({ msg: 'Unknown error' });
        }
        else {
            onError(data.error);
        }
    };
    var onSuccessCallback = function (data) {
        if (onSuccess == null) {
            return;
        }
        if (data != null && data.result != null) {
            if (data.error == null) {
                onSuccess(data.result);
            }
            else {
                onSuccess(data);
            }
        }
        else {
            onErrorCallback(data);
        }
    };

    $.ajax({
        url: url,
        type: 'POST',
        data: JSON.stringify(data),
        dataType: 'json',
        contentType: 'application/json',
        async: true,
        cache: false,
        success: function (data) {
            ShoppingPlugin.api.locale = data != null ? data.locale : null;
            onSuccessCallback(data);
        },
        error: function (data) {
            ShoppingPlugin.api.locale = data != null ? data.locale : null;
            onErrorCallback(data);
        }
    });
};

ShoppingPlugin.api.callAsync = function (url, data) {
    if (data == null) {
        data = {};
    }

    //init session token from URL if it's present
    if (ShoppingPlugin.helpers.hasQueryParam(ShoppingPlugin.constants.tokenUrlParam)) {
        data.session_token = ShoppingPlugin.helpers.getQueryParam(ShoppingPlugin.constants.tokenUrlParam);
    }

    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: 'POST',
            data: JSON.stringify(data),
            dataType: 'json',
            contentType: 'application/json',
            async: true,
            cache: false,
            success: function (data) {
                ShoppingPlugin.api.locale = data != null ? data.locale : null;
                if (data != null && data.result != null) {
                    if (data.error == null) {
                        resolve(data.result);
                    }
                    else {
                        resolve(data);
                    }
                }
                else {
                    reject(data);
                }
            },
            error: function (data) {
                ShoppingPlugin.api.locale = data != null ? data.locale : null;
                if (data == null || data.error == null || data.error.msg == null) {
                    reject({ msg: 'Unknown error' });
                }
                else {
                    reject(data.error);
                }
            }
        });
    });

};

/* Auth API */

ShoppingPlugin.api.auth = {};

ShoppingPlugin.api.auth.getSelf = function (callback, onError) {

    ShoppingPlugin.api.call('/api/auth/get_self', null, function (profile) {
        callback(profile);
    }, onError);
};

/* Cart API */

ShoppingPlugin.api.cart = {};

ShoppingPlugin.api.cart.getShippingCalculationOptions = function (callback, onError) {

    ShoppingPlugin.api.call('/api/cart/get_shipping_calculation_options', null, function (locations) {
        callback(locations);
    }, onError);
};

ShoppingPlugin.api.cart.getCart = function (callback, onError) {
    if (this.cartData == null) {
        this.cartData = {};
    }

    ShoppingPlugin.api.call('/api/cart/get_cart', { load_totals_from_cache: this.cartData.load_totals_from_cache }, function (data) {
        const cart = data.result != null ? data.result : data;
        ShoppingPlugin.api.cart.cartData = cart;
        callback(data);
    }, onError);
};

ShoppingPlugin.api.cart.getCartAsync = async function (force, loadTotalsFromCache) {
    if (!force && this.cartData) {
        return this.cartData;
    }

    this.cartData = await ShoppingPlugin.api.callAsync('/api/cart/get_cart',
        { load_totals_from_cache: loadTotalsFromCache });

    return this.cartData;
};

ShoppingPlugin.api.cart.updateShipping = function (callback, onError) {
    ShoppingPlugin.api.call('/api/cart/update_cart',
        {
            shipping_diff: this.cartData.shipping,
            distributor_limits_diff: this.cartData.distributor_limits,
            load_totals_from_cache: this.cartData.load_totals_from_cache
        },
        function (data) {
            const cart = data.result != null ? data.result : data;
            this.cartData = cart;
            if (callback != null) {
                callback(data);
            }
        }, onError);
};

ShoppingPlugin.api.cart.updateShippingAsync = async function (shipping) {
    await this.getCartAsync();

    shipping = $.extend(true, {}, this.cartData.shipping, shipping);

    return await ShoppingPlugin.api.callAsync('/api/cart/update_cart',
        {
            shipping_diff: shipping,
            distributor_limits_diff: this.cartData.distributor_limits,
            load_totals_from_cache: this.cartData.load_totals_from_cache
        });
};

ShoppingPlugin.api.cart.updateDistributorLimitsAsync = async function (limitsDiff) {
    await this.getCartAsync();

    return await ShoppingPlugin.api.callAsync('/api/cart/update_cart',
        {
            shipping_diff: this.cartData.shipping,
            distributor_limits_diff: limitsDiff,
            load_totals_from_cache: this.cartData.load_totals_from_cache
        });
};

ShoppingPlugin.api.cart.updateQuantity = async function (id, count) {
    const cartData = await this.getCartAsync();
    
    const item = cartData.cart.find(function (el) { return el.item_id === id; });
    const diff = count - item.quantity;

    this.cartData = await ShoppingPlugin.api.callAsync('/api/cart/update_cart',
        {
            items_diff: [
                {
                    item_id: id,
                    quantity_diff: diff
                }
            ]
        });

    return this.cartData;
};

ShoppingPlugin.api.cart.cartData = null;

/*UI*/

ShoppingPlugin.ui = {};


ShoppingPlugin.ui.enableButton = function (selector, enable, inactiveCss) {
    const button = $(selector);
    if (enable) {
        $(button).removeAttr('disabled');
        $(button).removeClass(inactiveCss);
    }
    else {
        $(button).attr('disabled', 'disabled');
        $(button).addClass(inactiveCss);
    }
};

ShoppingPlugin.ui.messageBox = function (opts) {
    const content = opts.content;
    var css = opts.css;
    if (css == null) {
        css = 'message-box-default';
    }
    var parentSelector = opts.parentSelector;
    if (parentSelector == null) {
        parentSelector = 'body';
    }
    if (opts.id != null) {
        $('#' + opts.id).remove();
    }
    const messageBox = $('<div class="message-box" id="{0}"></div>'.format(opts.id)).html(content).addClass(css).appendTo(parentSelector);
};


ShoppingPlugin.ui.popup = function (opts) {

    const content = opts.content;
    const popupCss = opts.css;
    const triggerSelector = opts.triggerSelector;
    var callback = opts.callback;
    const id = opts.id;

    const template =
        '<div class="popup-container" id="{0}" title="{1}">' +
            '<div class="popup-close"></div>' +
            '<div class="popup-content">{2}</div></div>';

    var popup = $(template.format(id, opts.title, content)).appendTo('body');

    $.ui.dialog.prototype._focusTabbable = $.noop;

    const dialog = popup.dialog(
        {
            autoOpen: false,
            dialogClass: popupCss,
            buttons: null,
            draggable: false,
            closeOnEscape: true,
            modal: true,
            resizable: false,
            close: function (event, ui) {
                $('body').css('height', 'auto');
                if (callback != null) {
                    callback();
                }
            }
        });

    if (triggerSelector != null) {
        if ($(triggerSelector).attr('href')) {
            $(triggerSelector).removeAttr('href');
        }
        $(triggerSelector).click(function () {

            const $body = $('body');
            const height = $body.outerHeight();
            $body.css('height', height + 'px');
            dialog.dialog('open');
        });
    }
    
    $(window).resize(function () {
        dialog.dialog('option', 'position', { my: 'center', at: 'center', of: window });
    });

    dialog.find('.popup-close').click(function () {
        dialog.dialog('close');
    });
    
    dialog.dialog('open');

    return dialog;
};
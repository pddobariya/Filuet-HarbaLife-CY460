$(document).ready(function () {

    $(document).on('click', '.amount .minus', function (e) {
        e.preventDefault();
        //for this code minus quantity button disabled
        $('.minus').prop('disabled', true);
        const $parent = $(this).closest('.amount'),
            $input = $parent.find('.input'),
            inputVal = parseFloat($input.val()),
            minimum = parseFloat($input.data('minimum')),
            step = parseFloat($input.data('step')),
            unit = $input.data('unit'),
            $buy = $(this).closest('.buy');
        $buy.find('.buy_btn').prop('disabled', false);

        if (inputVal > minimum + 1) {
            displayAjaxLoading(true);
            let oldquantity = inputVal;
            let newquantity = inputVal - step;
            updateproductincart(oldquantity, newquantity, $buy, $input, unit);
        }

        if (inputVal === 1) {

            displayAjaxLoading(true);

            $buy.find('.removefromcart').prop('checked', true);

            let form = $buy.find('.shopping-cart-form');
            let data = new FormData(form[0]);

            $.ajax({
                cache: false,
                url: "cart",
                type: "POST",
                data: data,
                contentType: false,
                processData: false,
                success: function (data, textStatus, jqXHR) {


                    displayAjaxLoading(false);
                    $buy.find('.buy_btn').removeClass('active');
                    $buy.find('.removefromcart').prop('checked', false);
                    $buy.find('.itemquantity').remove();
                    $('.minus').prop('disabled', false);

                    updateshoppingcartitems();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    displayAjaxLoading(false);
                    $buy.find('.removefromcart').prop('checked', false);
                    alert('Error');
                }
            });
        }
    });

    $(document).on('click', '.amount .plus', function (e) {
        e.preventDefault();
        //for this code plus quantity button disabled
        $('.plus').prop('disabled', true);

        const $parent = $(this).closest('.amount'),
            $input = $parent.find('.input'),
            inputVal = parseFloat($input.val()),
            maximum = parseFloat($input.data('maximum')),
            step = parseFloat($input.data('step')),
            unit = $input.data('unit'),
            $buy = $(this).closest('.buy');

        if (inputVal < maximum) {
            displayAjaxLoading(true);
            let oldquantity = inputVal;
            let newquantity = inputVal + step;
            updateproductincart(oldquantity, newquantity, $buy, $input, unit);
        }
    });

    $(document).on('change', '.amount .input', function (e) {
        e.preventDefault();

        const $parent = $(this).closest('.amount'),
            $input = $parent.find('.input'),
            inputVal = parseFloat($input.val()),
            maximum = parseFloat($input.data('maximum')),
            minimum = parseFloat($input.data('minimum')),
            $buy = $(this).closest('.buy');

        if (inputVal >= minimum && inputVal <= maximum) {
            displayAjaxLoading(true);
            let newquantity = inputVal;
            updateproductincart(1, newquantity, $buy, $input, '');
        }

        if (inputVal === 0) {

            displayAjaxLoading(true);

            $buy.find('.removefromcart').prop('checked', true);

            let form = $buy.find('.shopping-cart-form');
            let data = new FormData(form[0]);

            $.ajax({
                cache: false,
                url: "cart",
                type: "POST",
                data: data,
                contentType: false,
                processData: false,
                success: function (data, textStatus, jqXHR) {
                    displayAjaxLoading(false);
                    $buy.find('.buy_btn').removeClass('active');
                    $buy.find('.removefromcart').prop('checked', false);
                    $buy.find('.itemquantity').remove();

                    updateshoppingcartitems();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    displayAjaxLoading(false);
                    $buy.find('.removefromcart').prop('checked', false);
                    alert('Error');
                }
            });
        }
    });

    // Страница товара
    if ($('.product_info .images').length) {
        const productThumbs = new Swiper('.product_info .thumbs .swiper-container', {
            loop: false,
            speed: 500,
            watchSlidesVisibility: true,
            slideActiveClass: 'active',
            slideVisibleClass: 'visible',
            spaceBetween: 12,
            slidesPerView: 'auto',
            direction: 'vertical'
        })

        new Swiper('.product_info .big .swiper-container', {
            loop: false,
            speed: 500,
            watchSlidesVisibility: true,
            slideActiveClass: 'active',
            slideVisibleClass: 'visible',
            spaceBetween: 20,
            slidesPerView: 1,
            thumbs: {
                swiper: productThumbs
            },
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev'
            }
        })
    }
});

function updateflyoutcart() {
    $.ajax({
        url: "FiluetHerbalifePublic/GetFlyoutShoppingCart",
        type: "GET",
        success: function (data, textStatus, jqXHR) {
            $('#flyout_cart_root').html('' + data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Error');
        }
    });
}

function updateshoppingcartitems() {
    $.ajax({
        url: "FiluetHerbalifePublic/GetShoppingCartItems",
        type: "GET",
        success: function (data, textStatus, jqXHR) {
            //for this code plus minus quantity button enable
            $('.minus').prop('disabled', false);
            $('.plus').prop('disabled', false);

            $('#shopping_cart_quantity').html('(' + data.quantity + ')');
            updateflyoutcart();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Error');
        }
    });
}

function updatewishlistitems() {
    $.ajax({
        url: "FiluetHerbalifePublic/GetWishlistItems",
        type: "GET",
        success: function (data, textStatus, jqXHR) {
            $('.wishlist-qty').html('(' + data.quantity + ')');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert('Error');
        }
    });
}

function addproducttocart(addtocartlink, productId, buyBtn) {
    $(buyBtn).prop('disabled', true);
    displayAjaxLoading(true);
    var postData = {};
    addAntiForgeryToken(postData);
    $.ajax({
        cache: false,
        url: addtocartlink,
        type: "POST",
        dataType: "json",
        data: postData,
        success: function (data, textStatus, jqXHR) {
            updatewishlistitems();
            if (data.success) {
                $.ajax({
                    cache: false,
                    url: 'FiluetHerbalifePublic/GetUserCart',
                    type: "POST",
                    data: {
                        ProductId: productId
                    },
                    contentType: false,
                    processData: false,
                    success: function (data, textStatus, jqXHR) {
                        displayAjaxLoading(false);
                        if (data.length) {
                            for (let item of data) {
                                if (item.ProductId === productId) {
                                    $(buyBtn).closest('.buy').find('.removefromcart').val(item.Id);
                                    let $itemquantity = $(buyBtn).closest('.buy').find('.itemquantity');
                                    if (!$itemquantity.length) {
                                        $(buyBtn).closest('.buy').find('.shopping-cart-form').append('<input class="itemquantity" name="itemquantity' + item.Id + '" value="' + item.Quantity + '">');
                                    }
                                    break;
                                }
                            }
                            $(buyBtn).addClass('active');
                            updateshoppingcartitems();
                        } else {
                            console.warn('Method FiluetHerbalifePublic/GetUserCart returned empty data');
                            console.warn(data);
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        displayAjaxLoading(false);
                        $('.buy_btn').css({ 'background-color': '', 'color': '' });
                        $('.buy_btn').prop('disabled', false);
                        alert('Error');
                    }
                });
            } else {
                displayAjaxLoading(false);
                if (data.message) {
                   /* displayPopupNotificationwarning(data.message, 'warning', true);*/
                } else {
                    console.warn(`Method ${addtocartlink} is returned possible incorrect data. In the same time we don\'t have display warning message!`);
                    console.warn(data);
                }
            }
            $('.buy_btn').css({ 'background-color': '', 'color': '' });
            $('.buy_btn').prop('disabled', false);
            if (data.success !== true) {
                displayPopupNotificationwarning(data.message, 'warning', true);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            displayAjaxLoading(false);
            $('.buy_btn').css({ 'background-color': '', 'color': '' });
            $('.buy_btn').prop('disabled', false);
        }
    });
}

function displayPopupNotificationwarning(message, messagetype, modal) {
    //types: success, error, warning
    var container;
    if (messagetype == 'success') {
        //success
        container = $('#dialog-notifications-success');
    }
    else if (messagetype == 'error') {
        //error
        container = $('#dialog-notifications-error');
    }
    else if (messagetype == 'warning') {
        //warning
        container = $('#dialog-notifications-warning');
    }
    else {
        //other
        container = $('#dialog-notifications-success');
    }

    //we do not encode displayed message
    var htmlcode = '';
    if ((typeof message) == 'string') {
        htmlcode = '<p>' + message + '</p>';
    } else {
        for (var i = 0; i < message.length; i++) {
            htmlcode = htmlcode + '<p>' + message[i] + '</p>';
        }
    }

    container.html(htmlcode);

    var isModal = (modal ? true : false);
    var dialog = container.kendoDialog({
        width: "400px",
        title: container.attr('title'),
        closable: false,
        modal: isModal,
        content: htmlcode,
        actions: [
            { text: 'OK', primary: true, action: () => { container.hide(); } }
        ]
    });
    container.show();
    dialog.data("kendoDialog").open();
}

function updateproductincart(oldquantity, newquantity, $buy, $input, unit) {

    $buy.find('.itemquantity').val(newquantity);

    var form = $buy.find('.shopping-cart-form');
    var data = new FormData(form[0]);

    $.ajax({
        cache: false,
        url: "cart",
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        success: function (data, textStatus, jqXHR) {
            var items = data.Items;
            var hasWarnings = false;
            items.forEach(function (item) {
                var warnings = item.Warnings;
                if (warnings.length > 0) {
                    hasWarnings = true;
                    warnings.forEach(function (warning) {
                        displayPopupNotificationwarning(warning, 'warning', true);
                    });
                }
            });
            displayAjaxLoading(false);
            if (!hasWarnings)
            {
                $input.val(newquantity);
                updateshoppingcartitems();
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            displayAjaxLoading(false);
            $buy.find('.itemquantity').val(oldquantity);
            alert('Error');
        }
    });
}
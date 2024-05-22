// Herbalife
const shippingCountrySelect = '#shipping-country-select';
const deliveryTypeSelect = '#delivery-type-select';
const deliveryCitySelect = '#delivery-city-select';
const deliveryOperatorSelect = '#delivery-operator-select';
const pickAddressSelect = '#pick-address-select';
const deliveryAddressInput = '#delivery-address-input';

$(document).ready(function () {
    updateSelectize(deliveryTypeSelect, []);
    updateSelectize(deliveryCitySelect, []);
    updateSelectize(deliveryOperatorSelect, []);
    updateSelectize(pickAddressSelect, []);

    $(deliveryAddressInput).autocomplete({
        source: [],
        minLength: 0
    });

    $(document).on('click', deliveryAddressInput, function (e) {
        $(deliveryAddressInput).autocomplete("search", "");
    });

    $(document).on('change', shippingCountrySelect, function (e) {
        e.preventDefault();

        SetDeliveryTypeDefaults();

        ShippingCountry.onChange();
    });

    $(document).on('change', deliveryTypeSelect, function (e) {
        e.preventDefault();

        HideAndShowSelects();

        SetDeliveryCityDefaults()

        ShippingCountry.initializeCities();
    });

    $(document).on('change', deliveryCitySelect, function (e) {
        e.preventDefault();

        SetDeliveryOperatorDefaults();

        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());
        if (deliveryTypeId === 0) {
            ShippingCountry.InitializeAddresses();
        } else if (deliveryTypeId === 1 || deliveryTypeId === 2) {
            ShippingCountry.InitializeDeliveryOperators();
        }
    });

    $(document).on('change', deliveryOperatorSelect, function (e) {
        e.preventDefault();

        SetDeliveryAddressDefaults();

        ShippingCountry.InitializeAddresses();

        ShippingCountry.InitializePriceFromOperatorSelect();
    });

    $(document).on('change', pickAddressSelect, function (e) {
        e.preventDefault();

        SetDeliveryPriceDefaults()

        ShippingCountry.InitializePriceFromAddressSelect();
    });

    $(document).on('click', '#change_country_btn', function (e) {
        e.preventDefault();

        Fancybox.close([{
            all: '#unable_deliver_modal'
        }])
    });

    //получить страны
    ShippingCountry.getCountry();
    ShippingCountry.init();
});

var HerbalifeOnePageCheckout = {
    form: false,
    saveUrl: false,
    localized_data: false,
    phoneMask: '',

    init: function (form, saveUrl, localized_data, phoneMask) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.localized_data = localized_data;
        this.phoneMask = phoneMask;
    },

    validate: function () {
        let errorMessage = `${this.localized_data.SpecifyMethodError}:`;
        let findError = false;

        let deliveryCity = $(deliveryCitySelect).val();
        if (deliveryCity == null || deliveryCity == '') {
            findError = true;
            errorMessage += `\n${$('#delivery-city-label').html()}`;
        }

        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());
        if (deliveryTypeId === 0)       //SelfPickup
        {
            let selfpickupAddress = $(pickAddressSelect).val();
            if (selfpickupAddress == null || selfpickupAddress == '') {
                findError = true;
                errorMessage += `\n${$('#selfpickup-address-label').html()}`;
            }
        }
        else if (deliveryTypeId === 1)  //PickPoint
        {
            let deliveryOperator = $(deliveryOperatorSelect).val();
            if (deliveryOperator == null || deliveryOperator == '') {
                findError = true;
                errorMessage += `\n${$('#delivery-operator-label').html()}`;
            }

            let pickpointAddress = $(pickAddressSelect).val();
            if (pickpointAddress == null || pickpointAddress == '') {
                findError = true;
                errorMessage += `\n${$('#pickpoint-address-label').html()}`;
            }
        }
        else if (deliveryTypeId === 2)  //Delivery
        {
            let deliveryOperator = $(deliveryOperatorSelect).val();
            if (deliveryOperator == null || deliveryOperator == '') {
                findError = true;
                errorMessage += `\n${$('#delivery-operator-label').html()}`;
            }

            let deliveryAddress = $(deliveryAddressInput).val();
            if (deliveryAddress == '') {
                findError = true;
                errorMessage += `\n${$('#delivery-address-label').html()}`;
            }

            let deliveryPostcode = $('#delivery-postcode-input').val();
            if (deliveryPostcode == '') {
                findError = true;
                errorMessage += `\n${$('#delivery-postcode-label').html()}`;
            }
        }

        let receiverName = $('#receiver-name-input').val();
        if (receiverName == '') {
            findError = true;
            errorMessage += `\n${$('#receiver-name-label').html()}`;
        }

        let receiverPhone = $('#receiver-phone-input').val();
        if (receiverPhone == '') {
            findError = true;
            errorMessage += `\n${$('#receiver-phone-label').html()}`;
        }

        if (this.phoneMask.length) {
            var myRe = new RegExp(this.phoneMask);
            if (!myRe.test(receiverPhone)) {
                findError = true;
                errorMessage += `\n${$('#receiver-phone-format-label').html()}`;
            }
        }

        let receiverEmail = $('#receiver-email-input').val();
        let emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (receiverEmail == '') {
            findError = true;
            errorMessage += `\n${$('#receiver-email-label').html()}`;
        } else if (!emailRegex.test(receiverEmail)) {
            findError = true;
            errorMessage += `\nInvalid email address`;
        }

        if (findError) {
            alert(errorMessage);
            return false;
        }

        return true;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) {
            return;
        }

        if (this.validate()) {
            if (!ShippingCountry.onChange(isNextButton = true)) {
                return;
            }

            Checkout.setLoadWaiting('shipping-method');

            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: addAntiForgeryToken($(this.form).serialize()),
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    savePayment: function () {
        if (Checkout.loadWaiting !== false) {
            return;
        }


        Checkout.setLoadWaiting('shipping-method');

        $.ajax({
            cache: false,
            url: this.saveUrl,
            data: addAntiForgeryToken($(this.form).serialize()),
            type: "POST",
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });

    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        if (Checkout.loadWaiting !== 'shipping-method') {
            return;
        }

        if (response.redirect) {
            window.location.replace(response.redirect);
        }
        else {
            alert('Error');
        }
    }
};

var ShippingCountry = {
    deliveryTypes: null,
    salesCenters: null,
    deliveryOperatorsCities: null,
    deliveryOperators: null,
    autoPostOffices: null,
    deliveryAddresses: null,

    init() {
        this.deliveryTypes = this.GetDeliveryTypes();
        this.salesCenters = this.GetSalesCenters();
        this.deliveryOperatorsCities = this.GetDeliveryOperatorsCities();
        this.deliveryOperators = this.GetDeliveryOperators();
        this.autoPostOffices = this.GetAutoPostOffices();
        this.deliveryAddresses = this.GetDeliveryAddresses();
    },

    getCountry() {
        var currentObj = this;
        //получить страны
        $.ajax({
            cache: false,
            url: "api/shipping/get_shipping_calculation",
            type: "GET",
            data: {},
            success: function (data, textStatus, jqXHR) {
                if (data.options != null && data.options.length) {
                    var options = [];
                    for (let item of data.options) {
                        options.push({
                            id: item.id, displayOrder: item.displayOrder, isSelected: item.isSelected,
                            processingLocationCode: item.processingLocationCode, salesCenterId: item.salesCenterId,
                            warehouseCode: item.warehouseCode, countryCode: item.countryCode,
                            name: item.name, isSalesCenter: item.isSalesCenter
                        });
                    }

                    $(shippingCountrySelect).selectize({
                        valueField: ['id'],
                        labelField: 'name',
                        searchField: ['id', 'name'],
                        render: {
                            option: function (item, escape) {
                                return `<div class="list_item option" data-id=${item.id}
                                            data-displayorder="${item.displayOrder}"
                                            data-isselected="${item.isSelected}" 
                                            data-processinglocationcode="${item.processingLocationCode}"
                                            data-salescenterid="${item.salesCenterId}" 
                                            data-warehousecode="${item.warehouseCode}"
                                            data-name="${item.name}" 
                                            data-countrycode="${item.countryCode}"
                                            data-issalescenter="${item.isSalesCenter}"
                                        value="${item.id}">${item.name}</div>`;
                            },
                            item: function (item) {
                                return `<div class="item" data-id=${item.id}
                                            data-displayorder="${item.displayOrder}"
                                            data-isselected="${item.isSelected}" 
                                            data-processinglocationcode="${item.processingLocationCode}"
                                            data-salescenterid="${item.salesCenterId}" 
                                            data-warehousecode="${item.warehouseCode}"
                                            data-name="${item.name}" 
                                            data-countrycode="${item.countryCode}"
                                            data-issalescenter="${item.isSalesCenter}">
                                        ${item.name}
                                    </div>`;
                            }
                        }
                    });

                    var selectizeItem = $(shippingCountrySelect)[0].selectize;
                    selectizeItem.clear();
                    selectizeItem.clearOptions();
                    $.each(options, function (index, value) {
                        selectizeItem.addOption(value);
                        if (value.isSelected) {
                            selectizeItem.setValue(value.id, true);
                        }
                    });

                    //заполнить виды доставки
                    currentObj.initializeDeliveryTypes();
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });
    },

    onChange(isNextButton = false) {
        var currentObj = this;

        var selectizeItem = $(shippingCountrySelect)[0].selectize;
        var dataSet = selectizeItem.getItem(selectizeItem.getValue())[0].dataset;

        let data = {
            countryCode: dataSet.countrycode,
            displayOrder: parseInt(dataSet.displayorder),
            id: parseInt(dataSet.id),
            isSalesCenter: dataSet.issalescenter === 'true',
            isSelected: dataSet.isselected === 'true',
            name: dataSet.name,
            processingLocationCode: dataSet.processinglocationcode,
            salesCenterId: dataSet.salescenterid != null ? parseInt(dataSet.salescenterid) : null,
            warehouseCode: dataSet.warehousecode
        };

        let flag;
        //записать в базу новые параметры города
        $.ajax({
            async: false,
            type: "POST",
            url: "api/shipping/set_shipping_computation_option",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (data, textStatus, jqXHR) {

                if (data.distributorNull) {
                    alert("Distributor is null!");
                }
                if (!isNextButton) {
                    window.location.reload();
                    //заполнить виды доставки
                    //currentObj.initializeDeliveryTypes();
                }

                if (data.isCartValid && data.isCartValid.toLowerCase() == 'true') {
                    flag = true;
                }
                else {
                    let modal_desc = $('#unable_deliver_modal_hidden_desc').text().replace('{0}', data.skus);
                    $('#unable_deliver_modal_desc').text(modal_desc);

                    Fancybox.show([{
                        src: '#unable_deliver_modal',
                        type: 'inline'
                    }])

                    flag = false;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });

        return flag;
    },

    //Метод доставки
    initializeDeliveryTypes() {
        if (this.deliveryTypes == null)
            this.deliveryTypes = this.GetDeliveryTypes();

        if (this.deliveryTypes != null && this.deliveryTypes.length) {
            var options = [];
            for (let item of this.deliveryTypes) {
                options.push({
                    id: item.Id, name: item.TypeName
                });
            }

            updateSelectize(deliveryTypeSelect, options, 1);

            HideAndShowSelects();
            this.initializeCities();
        }
    },

    //Город доставки
    initializeCities() {
        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());

        if (deliveryTypeId === 0) {
            if (this.salesCenters == null)
                this.salesCenters = this.GetSalesCenters();

            if (this.salesCenters != null && this.salesCenters.length) {
                var options = [];
                for (let item of this.salesCenters) {
                    options.push({
                        id: item.Id, name: item.City
                    });
                }

                updateSelectize(deliveryCitySelect, options);
            }
        } else if (deliveryTypeId === 1 || deliveryTypeId === 2) {
            if (this.deliveryOperatorsCities == null)
                this.deliveryOperatorsCities = this.GetDeliveryOperatorsCities();

            if (this.deliveryOperatorsCities != null && this.deliveryOperatorsCities.length) {
                var options = [];
                for (let item of this.deliveryOperatorsCities) {
                    if (typeof item.DeliveryOperator_DeliveryType_DeliveryCity_Dependencies !== 'undefined') {
                        if (item.DeliveryOperator_DeliveryType_DeliveryCity_Dependencies.some(
                            d => d.DeliveryTypeId === deliveryTypeId)) {
                            options.push({
                                id: item.Id, name: item.CityName
                            });
                        }
                    }
                }

                updateSelectize(deliveryCitySelect, options);
            }
        }
    },

    //Вид доставки
    InitializeDeliveryOperators () {
        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());

        if (deliveryTypeId === 1 || deliveryTypeId === 2) {
            if (this.deliveryOperatorsCities == null)
                this.deliveryOperatorsCities = this.GetDeliveryOperatorsCities();

            if (this.deliveryOperators == null)
                this.deliveryOperators = this.GetDeliveryOperators();

            if (this.deliveryOperatorsCities != null && this.deliveryOperatorsCities.length
                && this.deliveryOperators != null && this.deliveryOperators.length) {
                let cityId = parseInt($(deliveryCitySelect).val());

                let selectedCity = this.deliveryOperatorsCities.find(city => city.Id === cityId);

                let operators = this.deliveryOperators.filter(operator =>
                    selectedCity?.DeliveryOperator_DeliveryType_DeliveryCity_Dependencies
                        .some(dodtdcd => dodtdcd.DeliveryOperatorId === operator.Id
                            && dodtdcd.DeliveryTypeId === deliveryTypeId))

                if (operators.length > 0) {
                    var options = [];
                    for (let item of operators) {
                        options.push({
                            id: item.Id, name: item.OperatorName
                        });
                    }
                    updateSelectize(deliveryOperatorSelect, options);
                }
            }
        }
    },

    //Адрес
    InitializeAddresses () {

        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());
        let selectedCityId = parseInt($(deliveryCitySelect).val());
        let selectedOperatorId = parseInt($(deliveryOperatorSelect).val());

        let addressOptions = null;

        if (deliveryTypeId === 0) {
            if (this.salesCenters == null)
                this.salesCenters = this.GetSalesCenters();

            if (this.salesCenters != null && this.salesCenters.length) {
                addressOptions = this.salesCenters.filter(sc => sc.Id === selectedCityId)
                    .map(sc => {
                        return { value: sc.Id.toString(), label: sc.Address };
                    });

                if (addressOptions !== null && addressOptions.length) {
                    var options = [];
                    for (let item of addressOptions) {
                        options.push({
                            id: item.value, name: item.label
                        });
                    }
                    updateSelectize(pickAddressSelect, options);
                }
            }
        }
        else if (deliveryTypeId === 1) {
            if (this.autoPostOffices == null)
                this.autoPostOffices = this.GetAutoPostOffices();

            if (this.autoPostOffices != null && this.autoPostOffices.length) {
                addressOptions = this.autoPostOffices.filter(apo => apo.DeliveryTypeId === deliveryTypeId &&
                    apo.DeliveryCityId === parseInt(selectedCityId ?? "0") &&
                    apo.DeliveryOperatorId === selectedOperatorId)
                    .map(apo => {
                        return { value: apo.Id.toString(), label: apo.Address };
                    });

                if (addressOptions !== null && addressOptions.length) {
                    var options = [];
                    for (let item of addressOptions) {
                        options.push({
                            id: item.value, name: item.label
                        });
                    }
                    updateSelectize(pickAddressSelect, options);
                }
            }
        }
        else if (deliveryTypeId === 2) {
            if (this.deliveryAddresses == null)
                this.deliveryAddresses = this.GetDeliveryAddresses();

            if (this.deliveryAddresses != null && this.deliveryAddresses.length) {
                addressOptions = this.deliveryAddresses.map(adr => {
                    return { value: adr, label: adr };
                });

                if (addressOptions !== null && addressOptions.length) {
                    let options = [];
                    for (let item of addressOptions) {
                        options.push({
                            label: item.value
                        });
                    }

                    $(deliveryAddressInput).autocomplete("option", "source", options);
                }
            }
        }
    },

    //Стоимость доставки
    InitializePriceFromAddressSelect() {
        var selectizeItem = $(pickAddressSelect)[0].selectize;
        var selectedElement = selectizeItem.getItem(selectizeItem.getValue())[0];
        if (selectedElement == null) return;
        var dataSet = selectedElement.dataset;

        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());
        if (deliveryTypeId === 0) {
            if (this.salesCenters == null)
                this.salesCenters = this.GetSalesCenters();

            if (this.salesCenters != null && this.salesCenters.length) {
                let scSelected = this.salesCenters.find(sc => sc.Id === parseInt(dataSet.id));

                $('#delivery-price-div').text('€ ' + (scSelected?.Price ?? '0.00'));
                $('#delivery-priceId-input').val('');
            }
        }
        else if (deliveryTypeId === 1) {
            if (this.autoPostOffices == null)
                this.autoPostOffices = this.GetAutoPostOffices();

            if (this.autoPostOffices != null && this.autoPostOffices.length) {
                let apo = this.autoPostOffices.find(apoDto => apoDto.Id === parseInt(dataSet.id));

                let dodtdcd = apo?.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId;
                if (dodtdcd) {
                    let operatorPrice = this.GetOperatorPrice(dodtdcd);
                    $('#delivery-price-div').text('€ ' + (operatorPrice?.DeliveryPrise ?? '0.00'));
                    $('#delivery-priceId-input').val(operatorPrice?.Id ?? '');
                }
            }
        }
    },

    //Стоимость доставки
    InitializePriceFromOperatorSelect () {
        let deliveryTypeId = parseInt($(deliveryTypeSelect).val());
        if (deliveryTypeId === 2) {
            if (this.deliveryOperatorsCities == null)
                this.deliveryOperatorsCities = this.GetDeliveryOperatorsCities();

            if (this.deliveryOperatorsCities != null && this.deliveryOperatorsCities.length) {
                let selectedCityId = $(deliveryCitySelect).val();

                var selectizeItem = $(deliveryOperatorSelect)[0].selectize;
                var selectedElement = selectizeItem.getItem(selectizeItem.getValue())[0];
                if (selectedElement == null) return;
                var dataSet = selectedElement.dataset;

                let dodtdcd = this.deliveryOperatorsCities.find(c => c.Id === parseInt(selectedCityId ?? "0"))
                    ?.DeliveryOperator_DeliveryType_DeliveryCity_Dependencies.find((dodtdc) => {
                        return dodtdc.DeliveryOperatorId == parseInt(dataSet.id);
                    })?.Id;

                if (dodtdcd) {
                    let operatorPrice = this.GetOperatorPrice(dodtdcd);
                    $('#delivery-price-div').text('€ ' + (operatorPrice?.DeliveryPrise ?? '0.00'));
                    $('#delivery-priceId-input').val(operatorPrice?.Id ?? '');
                }
            }
        }
    },


    GetDeliveryTypes() {
        let deliveryTypes = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetDeliveryTypes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                if (data != null && data.length) {
                    data.sort(function (a, b) {
                        return a.Id - b.Id;
                    })
                    deliveryTypes = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });
        return deliveryTypes;
    },

    GetSalesCenters () {
        let salesCenters = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetSalesCenters",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                console.log(data, textStatus, jqXHR)
                if (data != null && data.length) {
                    salesCenters = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });
        return salesCenters;
    },

    GetDeliveryOperatorsCities () {
        let deliveryOperatorsCities = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetDeliveryOperatorsCities",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                   if (data != null && data.length) {
                    deliveryOperatorsCities = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });

        return deliveryOperatorsCities;
    },

    GetDeliveryOperators () {
        let deliveryOperators = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetDeliveryOperators",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                if (data != null && data.length) {
                    deliveryOperators = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });

        return deliveryOperators;
    },

    GetDeliveryAddresses () {
        let deliveryAddresses = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetDeliveryAddresses",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                if (data != null && data.length) {
                    deliveryAddresses = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });

        return deliveryAddresses;
    },

    GetAutoPostOffices () {
        let autoPostOffices = null;
        $.ajax({
            async: false,
            type: "POST",
            url: "Plugins/Delivery/GetAutoPostOffices",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({}),
            success: function (data, textStatus, jqXHR) {
                if (data != null && data.length) {
                    autoPostOffices = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });

        return autoPostOffices;
    },

    GetOperatorPrice (dodtdcd) {
        let operatorPrice = null;

        $.ajax({
            async: false,
            type: "Post",
            url: "Plugins/Delivery/GetOperatorPrice?dodtdcd=" + dodtdcd,
            headers: {
                'Content-Type': 'application/json'
            },
            data: null,
            success: function (data, textStatus, jqXHR) {
                if (data != null && data.Id ) {
                    operatorPrice = data;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {

            }
        });

        return operatorPrice;
    }
}

function SetDeliveryTypeDefaults() {
    SetDeliveryCityDefaults();
    updateSelectize(deliveryTypeSelect, []);
}
function SetDeliveryCityDefaults() {
    SetDeliveryOperatorDefaults();
    updateSelectize(deliveryCitySelect, []);
}
function SetDeliveryOperatorDefaults() {
    SetDeliveryAddressDefaults();
    updateSelectize(deliveryOperatorSelect, []);
}
function SetDeliveryAddressDefaults() {
    SetDeliveryPriceDefaults();
    updateSelectize(pickAddressSelect, []);
}
function SetDeliveryPriceDefaults() {
    $('#delivery-price-div').text('€ 0.00');
    $('#delivery-priceId-input').val('');
}

function HideAndShowSelects() {

    $('#delivery-operator-cont').hide();

    $('#pick-address-cont').hide();
    $('#selfpickup-address-label').hide();
    $('#pickpoint-address-label').hide();

    $('#delivery-address-cont').hide();
    $('#delivery-postcode-cont').hide();

    let deliveryTypeId = parseInt($(deliveryTypeSelect).val());

    if (deliveryTypeId === 0)       //SelfPickup
    {
        $('#pick-address-cont').show();
        $('#selfpickup-address-label').show();
    }
    else if (deliveryTypeId === 1)  //PickPoint
    {
        $('#delivery-operator-cont').show();
        $('#pick-address-cont').show();
        $('#pickpoint-address-label').show();
    }
    else if (deliveryTypeId === 2)  //Delivery
    {
        $('#delivery-operator-cont').show();
        $('#delivery-address-cont').show();
        $('#delivery-postcode-cont').show();
    }
}

function updateSelectize(selectElement, options, defaultValue = 0) {
    $(selectElement).selectize({
        valueField: ['id'],
        labelField: 'name',
        searchField: ['id', 'name'],
        render: {
            option: function (item, escape) {
                return `<div class="option" data-id=${item.id} 
                                        value="${item.id}">${item.name}</div>`;
            },
            item: function (item) {
                return `<div class="item" data-id=${item.id}>
                                        ${item.name}
                                    </div>`;
            }
        }
    });

    var selectizeItem = $(selectElement)[0].selectize;
    selectizeItem.clear();
    selectizeItem.clearOptions();
    $.each(options, function (index, value) {
        selectizeItem.addOption(value);
        if (defaultValue && defaultValue == 1) {
            selectizeItem.setValue(value.id, true);
            defaultValue++;
        }
    })
}

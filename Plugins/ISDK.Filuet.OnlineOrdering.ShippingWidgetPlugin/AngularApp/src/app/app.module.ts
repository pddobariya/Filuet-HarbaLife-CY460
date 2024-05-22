import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ApplicationRef } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import { InputMaskModule } from 'primeng/inputmask';
import { ListboxModule } from 'primeng/listbox';
import { InputTextModule } from 'primeng/inputtext';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { HttpClientModule } from '@angular/common/http';
import { LoaderSpinnerComponent } from './components/common/loader-spinner/loader-spinner.component';
import { ShippingFormComponent } from './components/shipping-form/shipping-form.component';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { LocalizerPipe } from './pipes/localizer.pipe';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { DropdownModule } from 'primeng/dropdown';
import { BlockUIModule } from 'primeng/blockui';
import { RadioButtonModule } from 'primeng/radiobutton';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CheckoutShippingMethodTopWidgetComponent } from './components/checkout-shipping-method-top-widget/checkout-shipping-method-top-widget.component';
import { ShippingCountrySelectComponent } from './components/shipping-country-select/shipping-country-select.component';
import { ShippingMethodSelectComponent } from './components/shipping-method-select/shipping-method-select.component';
import { OrderMonthSelectComponent } from './components/order-month-select/order-month-select.component';
import { OrderSummaryContentBeforeWidgetComponent } from './components/order-summary-content-before-widget/order-summary-content-before-widget.component';
import { NopViewComponentNames } from './constants/generated/NopViewComponentNames';
import { TooltipModule } from 'primeng/tooltip';
import { PanelModule } from 'primeng/panel';

@NgModule({
  declarations: [
    LoaderSpinnerComponent,
    ShippingFormComponent,
    LocalizerPipe,
    CheckoutShippingMethodTopWidgetComponent,
    ShippingCountrySelectComponent,
    ShippingMethodSelectComponent,
    OrderMonthSelectComponent,
    OrderSummaryContentBeforeWidgetComponent
  ],
  imports: [
    BrowserModule,
    AccordionModule,
    InputMaskModule,
    ListboxModule,
    InputTextModule,
    BrowserAnimationsModule,
    FormsModule,
    ButtonModule,
    DialogModule,
    HttpClientModule,
    MessagesModule,
    MessageModule,
    ReactiveFormsModule,
    InputTextareaModule,
    DropdownModule,
    BlockUIModule,
    RadioButtonModule,
    OverlayPanelModule,
    TooltipModule,
    PanelModule
  ],
  providers: [],
  entryComponents: [
    CheckoutShippingMethodTopWidgetComponent,
    OrderSummaryContentBeforeWidgetComponent
  ]
})
export class AppModule {
  ngDoBootstrap(appRef: ApplicationRef) {
    let options = {};
    options[NopViewComponentNames.CheckoutShippingMethodTopWidget] = {
      selector: 'app-checkout-shipping-method-top-widget',
      componentClass: CheckoutShippingMethodTopWidgetComponent
    };
    options[NopViewComponentNames.OrderSummaryContentBeforeWidget] = {
      selector: 'app-order-summary-content-before-widget',
      componentClass: OrderSummaryContentBeforeWidgetComponent
    };

    for (var nopViewComponentName in options) {
      let option = options[nopViewComponentName];
      const nopViewComponentElements = document.getElementsByClassName(nopViewComponentName);
      if (nopViewComponentElements.length > 0) {
        const ngComponentElement = document.createElement(option.selector);
        nopViewComponentElements[0].appendChild(ngComponentElement);
        appRef.bootstrap(option.componentClass);
      }
    }
  }
}

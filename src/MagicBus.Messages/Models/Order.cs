using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace MagicBus.Messages.Models
{
    
    
    //public class Order    {

    //    [JsonProperty("id")]
    //    public string Id { get; set; } 

    //    [JsonProperty("email")]
    //    public string Email { get; set; } 

    //    [JsonProperty("closed_at")]
    //    public DateTime? ClosedAt { get; set; } 

    //    [JsonProperty("created_at")]
    //    public DateTime CreatedAt { get; set; } 

    //    [JsonProperty("updated_at")]
    //    public DateTime UpdatedAt { get; set; } 

    //    [JsonProperty("number")]
    //    public string Number { get; set; } 

    //    [JsonProperty("note")]
    //    public string Note { get; set; } 

    //    [JsonProperty("token")]
    //    public string Token { get; set; } 

    //    [JsonProperty("gateway")]
    //    public string Gateway { get; set; } 

    //    [JsonProperty("test")]
    //    public bool Test { get; set; } 

    //    [JsonProperty("total_price")]
    //    public decimal? TotalPrice { get; set; } 

    //    [JsonProperty("subtotal_price")]
    //    public decimal? SubtotalPrice { get; set; } 

    //    [JsonProperty("total_weight")]
    //    public decimal? TotalWeight { get; set; } 

    //    [JsonProperty("total_tax")]
    //    public decimal? TotalTax { get; set; } 

    //    [JsonProperty("taxes_included")]
    //    public bool TaxesIncluded { get; set; } 

    //    [JsonProperty("currency")]
    //    public string Currency { get; set; } 

    //    [JsonProperty("financial_status")]
    //    public string FinancialStatus { get; set; } 

    //    [JsonProperty("confirmed")]
    //    public bool Confirmed { get; set; } 

    //    [JsonProperty("total_discounts")]
    //    public decimal? TotalDiscounts { get; set; } 

    //    [JsonProperty("total_line_items_price")]
    //    public decimal? TotalLineItemsPrice { get; set; } 

    //    [JsonProperty("cart_token")]
    //    public string CartToken { get; set; } 

    //    [JsonProperty("buyer_accepts_marketing")]
    //    public bool BuyerAcceptsMarketing { get; set; } 

    //    [JsonProperty("name")]
    //    public string Name { get; set; } 

    //    [JsonProperty("referring_site")]
    //    public string ReferringSite { get; set; } 

    //    [JsonProperty("landing_site")]
    //    public string LandingSite { get; set; } 

    //    [JsonProperty("cancelled_at")]
    //    public DateTime? CancelledAt { get; set; } 

    //    [JsonProperty("cancel_reason")]
    //    public string CancelReason { get; set; } 

    //    [JsonProperty("total_price_usd")]
    //    public decimal? TotalPriceUsd { get; set; } 

    //    [JsonProperty("checkout_token")]
    //    public string CheckoutToken { get; set; } 

    //    [JsonProperty("reference")]
    //    public string Reference { get; set; } 

    //    [JsonProperty("user_id")]
    //    public string UserId { get; set; } 

    //    [JsonProperty("location_id")]
    //    public string LocationId { get; set; } 

    //    [JsonProperty("source_identifier")]
    //    public string SourceIdentifier { get; set; } 

    //    [JsonProperty("source_url")]
    //    public string SourceUrl { get; set; } 

    //    [JsonProperty("processed_at")]
    //    public DateTime? ProcessedAt { get; set; } 

    //    [JsonProperty("device_id")]
    //    public string DeviceId { get; set; } 

    //    [JsonProperty("phone")]
    //    public string Phone { get; set; } 

    //    [JsonProperty("customer_locale")]
    //    public string CustomerLocale { get; set; } 

    //    [JsonProperty("app_id")]
    //    public string AppId { get; set; } 

    //    [JsonProperty("browser_ip")]
    //    public string BrowserIp { get; set; } 

    //    [JsonProperty("landing_site_ref")]
    //    public string LandingSiteRef { get; set; } 

    //    [JsonProperty("order_number")]
    //    public string OrderNumber { get; set; } 

    //    [JsonProperty("discount_applications")]
    //    public List<DiscountApplication> DiscountApplications { get; set; } 

    //    [JsonProperty("discount_codes")]
    //    public List<string> DiscountCodes { get; set; } 

    //    [JsonProperty("note_attributes")]
    //    public List<string> NoteAttributes { get; set; } 

    //    [JsonProperty("payment_gateway_names")]
    //    public List<string> PaymentGatewayNames { get; set; } 

    //    [JsonProperty("processing_method")]
    //    public string ProcessingMethod { get; set; } 

    //    [JsonProperty("checkout_id")]
    //    public string CheckoutId { get; set; } 

    //    [JsonProperty("source_name")]
    //    public string SourceName { get; set; } 

    //    [JsonProperty("fulfillment_status")]
    //    public string FulfillmentStatus { get; set; } 

    //    [JsonProperty("tax_lines")]
    //    public List<TaxLine> TaxLines { get; set; } 

    //    [JsonProperty("tags")]
    //    public string Tags { get; set; } 

    //    [JsonProperty("contact_email")]
    //    public string ContactEmail { get; set; } 

    //    [JsonProperty("order_status_url")]
    //    public string OrderStatusUrl { get; set; } 

    //    [JsonProperty("presentment_currency")]
    //    public string PresentmentCurrency { get; set; } 

    //    [JsonProperty("total_line_items_price_set")]
    //    public AmountSet TotalLineItemsPriceSet { get; set; } 

    //    [JsonProperty("total_discounts_set")]
    //    public AmountSet TotalDiscountsSet { get; set; } 

    //    [JsonProperty("total_shipping_price_set")]
    //    public AmountSet TotalShippingPriceSet { get; set; } 

    //    [JsonProperty("subtotal_price_set")]
    //    public AmountSet SubtotalPriceSet { get; set; } 

    //    [JsonProperty("total_price_set")]
    //    public AmountSet TotalPriceSet { get; set; } 

    //    [JsonProperty("total_tax_set")]
    //    public AmountSet TotalTaxSet { get; set; } 

    //    [JsonProperty("line_items")]
    //    public List<LineItem> LineItems { get; set; } 

    //    [JsonProperty("fulfillments")]
    //    public List<object> Fulfillments { get; set; } 

    //    [JsonProperty("refunds")]
    //    public List<object> Refunds { get; set; } 

    //    [JsonProperty("total_tip_received")]
    //    public string TotalTipReceived { get; set; } 

    //    [JsonProperty("original_total_duties_set")]
    //    public object OriginalTotalDutiesSet { get; set; } 

    //    [JsonProperty("current_total_duties_set")]
    //    public object CurrentTotalDutiesSet { get; set; } 

    //    [JsonProperty("admin_graphql_api_id")]
    //    public string AdminGraphqlApiId { get; set; } 

    //    [JsonProperty("shipping_lines")]
    //    public List<ShippingLine> ShippingLines { get; set; } 

    //    [JsonProperty("billing_address")]
    //    public Address BillingAddress { get; set; } 

    //    [JsonProperty("shipping_address")]
    //    public Address ShippingAddress { get; set; } 

    //    [JsonProperty("customer")]
    //    public Customer Customer { get; set; } 

    //}

    //public class TaxLine
    //{
    //    [JsonProperty("title")]
    //    public string Title { get; set; }
    //    [JsonProperty("price")]
    //    public decimal Price { get; set; }
    //    [JsonProperty("rate")]
    //    public decimal Rate  { get; set; }
    //    [JsonProperty("price_set")]
    //    public AmountSet PriceSet { get; set; }
    //}


    //    public class DiscountApplication    {

    //    [JsonProperty("type")]
    //    public string Type { get; set; } 

    //    [JsonProperty("value")]
    //    public string Value { get; set; } 

    //    [JsonProperty("value_type")]
    //    public string ValueType { get; set; } 

    //    [JsonProperty("allocation_method")]
    //    public string AllocationMethod { get; set; } 

    //    [JsonProperty("target_selection")]
    //    public string TargetSelection { get; set; } 

    //    [JsonProperty("target_type")]
    //    public string TargetType { get; set; } 

    //    [JsonProperty("description")]
    //    public string Description { get; set; } 

    //    [JsonProperty("title")]
    //    public string Title { get; set; } 

    //}

    //public class CurrencyAmount    {

    //    [JsonProperty("amount")]
    //    public string Amount { get; set; } 

    //    [JsonProperty("currency_code")]
    //    public string CurrencyCode { get; set; } 

    //}
    
    
   
    //public class AmountSet    {

    //    [JsonProperty("shop_money")]
    //    public CurrencyAmount ShopMoney { get; set; } 

    //    [JsonProperty("presentment_money")]
    //    public CurrencyAmount PresentmentMoney { get; set; } 

    //}

    //public class DiscountAllocation    {

    //    [JsonProperty("amount")]
    //    public string Amount { get; set; } 

    //    [JsonProperty("discount_application_index")]
    //    public int DiscountApplicationIndex { get; set; } 

    //    [JsonProperty("amount_set")]
    //    public AmountSet AmountSet { get; set; } 

    //}

    //public class LineItem    {

    //    [JsonProperty("id")]
    //    public string Id { get; set; } 

    //    [JsonProperty("variant_id")]
    //    public string VariantId { get; set; } 

    //    [JsonProperty("title")]
    //    public string Title { get; set; } 

    //    [JsonProperty("quantity")]
    //    public int Quantity { get; set; } 

    //    [JsonProperty("sku")]
    //    public string Sku { get; set; } 

    //    [JsonProperty("variant_title")]
    //    public string VariantTitle { get; set; } 

    //    [JsonProperty("vendor")]
    //    public string Vendor { get; set; } 

    //    [JsonProperty("fulfillment_service")]
    //    public string FulfillmentService { get; set; } 

    //    [JsonProperty("product_id")]
    //    public string ProductId { get; set; } 

    //    [JsonProperty("requires_shipping")]
    //    public bool RequiresShipping { get; set; } 

    //    [JsonProperty("taxable")]
    //    public bool Taxable { get; set; } 

    //    [JsonProperty("gift_card")]
    //    public bool GiftCard { get; set; } 

    //    [JsonProperty("name")]
    //    public string Name { get; set; } 

    //    [JsonProperty("variant_inventory_management")]
    //    public string VariantInventoryManagement { get; set; } 

    //    [JsonProperty("properties")]
    //    public List<object> Properties { get; set; } 

    //    [JsonProperty("product_exists")]
    //    public bool ProductExists { get; set; } 

    //    [JsonProperty("fulfillable_quantity")]
    //    public int FulfillableQuantity { get; set; } 

    //    [JsonProperty("grams")]
    //    public int Grams { get; set; } 

    //    [JsonProperty("price")]
    //    public string Price { get; set; } 

    //    [JsonProperty("total_discount")]
    //    public string TotalDiscount { get; set; } 

    //    [JsonProperty("fulfillment_status")]
    //    public object FulfillmentStatus { get; set; } 

    //    [JsonProperty("price_set")]
    //    public AmountSet PriceSet { get; set; } 

    //    [JsonProperty("total_discount_set")]
    //    public AmountSet TotalDiscountSet { get; set; } 

    //    [JsonProperty("discount_allocations")]
    //    public List<DiscountAllocation> DiscountAllocations { get; set; } 

    //    [JsonProperty("duties")]
    //    public List<object> Duties { get; set; } 

    //    [JsonProperty("admin_graphql_api_id")]
    //    public string AdminGraphqlApiId { get; set; } 

    //    [JsonProperty("tax_lines")]
    //    public List<object> TaxLines { get; set; } 

    //}

   

    //public class ShippingLine    {

    //    [JsonProperty("id")]
    //    public long Id { get; set; } 

    //    [JsonProperty("title")]
    //    public string Title { get; set; } 

    //    [JsonProperty("price")]
    //    public string Price { get; set; } 

    //    [JsonProperty("code")]
    //    public object Code { get; set; } 

    //    [JsonProperty("source")]
    //    public string Source { get; set; } 

    //    [JsonProperty("phone")]
    //    public object Phone { get; set; } 

    //    [JsonProperty("requested_fulfillment_service_id")]
    //    public object RequestedFulfillmentServiceId { get; set; } 

    //    [JsonProperty("delivery_category")]
    //    public object DeliveryCategory { get; set; } 

    //    [JsonProperty("carrier_identifier")]
    //    public object CarrierIdentifier { get; set; } 

    //    [JsonProperty("discounted_price")]
    //    public string DiscountedPrice { get; set; } 

    //    [JsonProperty("price_set")]
    //    public AmountSet PriceSet { get; set; } 

    //    [JsonProperty("discounted_price_set")]
    //    public AmountSet DiscountedPriceSet { get; set; } 

    //    [JsonProperty("discount_allocations")]
    //    public List<object> DiscountAllocations { get; set; } 

    //    [JsonProperty("tax_lines")]
    //    public List<object> TaxLines { get; set; } 

    //}
    
    
    //public class Address    {

    //    [JsonProperty("id")]
    //    public long Id { get; set; } 

    //    [JsonProperty("customer_id")]
    //    public long CustomerId { get; set; } 

    //    [JsonProperty("first_name")]
    //    public object FirstName { get; set; } 

    //    [JsonProperty("last_name")]
    //    public object LastName { get; set; } 

    //    [JsonProperty("company")]
    //    public object Company { get; set; } 

    //    [JsonProperty("address1")]
    //    public string Address1 { get; set; } 

    //    [JsonProperty("address2")]
    //    public object Address2 { get; set; } 

    //    [JsonProperty("city")]
    //    public string City { get; set; } 

    //    [JsonProperty("province")]
    //    public string Province { get; set; } 

    //    [JsonProperty("country")]
    //    public string Country { get; set; } 

    //    [JsonProperty("zip")]
    //    public string Zip { get; set; } 

    //    [JsonProperty("phone")]
    //    public string Phone { get; set; } 

    //    [JsonProperty("name")]
    //    public string Name { get; set; } 

    //    [JsonProperty("province_code")]
    //    public string ProvinceCode { get; set; } 

    //    [JsonProperty("country_code")]
    //    public string CountryCode { get; set; } 

    //    [JsonProperty("country_name")]
    //    public string CountryName { get; set; } 

    //    [JsonProperty("default")]
    //    public bool Default { get; set; } 

    //}

    //public class Customer    {

    //    [JsonProperty("id")]
    //    public long Id { get; set; } 

    //    [JsonProperty("email")]
    //    public string Email { get; set; } 

    //    [JsonProperty("accepts_marketing")]
    //    public bool AcceptsMarketing { get; set; } 

    //    [JsonProperty("created_at")]
    //    public object CreatedAt { get; set; } 

    //    [JsonProperty("updated_at")]
    //    public object UpdatedAt { get; set; } 

    //    [JsonProperty("first_name")]
    //    public string FirstName { get; set; } 

    //    [JsonProperty("last_name")]
    //    public string LastName { get; set; } 

    //    [JsonProperty("orders_count")]
    //    public int OrdersCount { get; set; } 

    //    [JsonProperty("state")]
    //    public string State { get; set; } 

    //    [JsonProperty("total_spent")]
    //    public string TotalSpent { get; set; } 

    //    [JsonProperty("last_order_id")]
    //    public object LastOrderId { get; set; } 

    //    [JsonProperty("note")]
    //    public object Note { get; set; } 

    //    [JsonProperty("verified_email")]
    //    public bool VerifiedEmail { get; set; } 

    //    [JsonProperty("multipass_identifier")]
    //    public object MultipassIdentifier { get; set; } 

    //    [JsonProperty("tax_exempt")]
    //    public bool TaxExempt { get; set; } 

    //    [JsonProperty("phone")]
    //    public object Phone { get; set; } 

    //    [JsonProperty("tags")]
    //    public string Tags { get; set; } 

    //    [JsonProperty("last_order_name")]
    //    public object LastOrderName { get; set; } 

    //    [JsonProperty("currency")]
    //    public string Currency { get; set; } 

    //    [JsonProperty("accepts_marketing_updated_at")]
    //    public object AcceptsMarketingUpdatedAt { get; set; } 

    //    [JsonProperty("marketing_opt_in_level")]
    //    public object MarketingOptInLevel { get; set; } 

    //    [JsonProperty("admin_graphql_api_id")]
    //    public string AdminGraphqlApiId { get; set; } 

    //    [JsonProperty("default_address")]
    //    public Address DefaultAddress { get; set; } 

    //}

    



}
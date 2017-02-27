AppData folder has 3 csf file for maintain regular, sale, additional product discount, group promotional price of Products.

# Product.csv
    this file has 4 column 
    ID // Primary Key
    Product // product name
    Price // regular price
    Discount // discount in percentage 
    
# AdditionalProductDiscount.csv
    ID // primary key
    ProductId // foreign key for product
    Qty //  additional quanity of product
    Discount // discount in percentage 
    IsActive // true or false , letter we can use start date and end date for get active discounts

# GroupPromotionalPrice.csv
    ID // Primary key
     ProductId // foreign key of product
     Qty // product qty 
     Amount // group product amount, like 3 apple in 2$
    
    
 # General   
     we are using 2 promotions one is discount on additional product and second group prmotional.
     if we have discount in both for cart product then we will apply benefitial discount on product. 
     if somewhere we should apply both discount then we are appling it.
 
 
 # Enhancements.
    As it was develop in short time so we can enhance in arhitecture and need to refine code for addition discount types.
 
 
    
    

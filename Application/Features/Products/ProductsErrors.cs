﻿using Application.Shared.Response;
namespace Application.Features.Products;
public static class ProductsErrors
{
    public static readonly Error Conflict = Error.Conflict(
    "Products.SameProduct", "Product already exist");
}



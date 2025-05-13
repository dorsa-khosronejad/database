using System;
using System.Collections.Generic;

namespace RecipeDatabaseApp.Entities;

public partial class Recipeingredient
{
    public int Recipeid { get; set; }

    public int Ingredientid { get; set; }

    public decimal Quantity { get; set; }

    public virtual Ingredient Ingredient { get; set; } = null!;

    public virtual Recipe Recipe { get; set; } = null!;
}

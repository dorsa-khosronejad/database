using System;
using System.Collections.Generic;

namespace RecipeDatabaseApp.Entities;

public partial class Ingredient
{
    public int Ingredientid { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Recipeingredient> Recipeingredients { get; set; } = new List<Recipeingredient>();
}

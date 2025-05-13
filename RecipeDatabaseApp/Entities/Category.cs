using System;
using System.Collections.Generic;

namespace RecipeDatabaseApp.Entities;

public partial class Category
{
    public int Categoryid { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}

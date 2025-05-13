using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeDatabaseApp.Entities;

namespace RecipeDatabaseApp.Controllers
{
    public class RecipeController
    {
        private readonly RecipeDbContext _dbContext;

        public RecipeController(RecipeDbContext context)
        {
            _dbContext = context;
        }

        public async Task ListAllRecipes()
        {
            var recipes = await _dbContext.Recipes
                .Include(r => r.Recipeingredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.Categories)
                .ToListAsync();

            Console.WriteLine("\n=== All Recipes ===");
            foreach (var r in recipes)
            {
                Console.WriteLine($"[{r.Recipeid}] {r.Title}");
                Console.WriteLine("  Ingredients:");
                foreach (var ri in r.Recipeingredients)
                    Console.WriteLine($"    - {ri.Ingredient.Name}: {ri.Quantity} {ri.Ingredient.Unit}");

                Console.WriteLine("  Categories: " + string.Join(", ", r.Categories.Select(c => c.Name)));
                Console.WriteLine();
            }
        }

        public async Task AddCategoryToRecipe()
        {
            Console.Write("Recipe ID: ");
            if (!int.TryParse(Console.ReadLine(), out var rid)) return;
            Console.Write("Category ID: ");
            if (!int.TryParse(Console.ReadLine(), out var cid)) return;

            var recipe = await _dbContext.Recipes
                .Include(r => r.Categories)
                .FirstOrDefaultAsync(r => r.Recipeid == rid);
            var category = await _dbContext.Categories.FindAsync(cid);
            if (recipe == null || category == null)
            {
                Console.WriteLine("Invalid recipe or category ID.");
                return;
            }

            recipe.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Category added to recipe.");
        }

        public async Task AddNewIngredient()
        {
            Console.Write("Name: ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Unit (e.g. g, tsp): ");
            var unit = Console.ReadLine()?.Trim() ?? string.Empty;

            var ing = new Ingredient { Name = name, Unit = unit };
            _dbContext.Ingredients.Add(ing);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Created Ingredient #{ing.Ingredientid}.");
        }

      public async Task AddNewRecipe()
        {
            Console.Write("Title: ");
            var title = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Description: ");
            var desc = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Instructions: ");
            var instr = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Prep time (minutes): ");
            int.TryParse(Console.ReadLine(), out var prep);
            Console.Write("Cook time (minutes): ");
            int.TryParse(Console.ReadLine(), out var cook);
            Console.Write("Serving size: ");
            int.TryParse(Console.ReadLine(), out var serve);

            var recipe = new Recipe
            {
                Title = title,
                Description = desc,
                Instructions = instr,
                Preptime = prep,
                Cooktime = cook,
                Servingsize = serve
            };

            _dbContext.Recipes.Add(recipe);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine($"Created Recipe #{recipe.Recipeid}.");
        }

        public async Task UpdateRecipe()
        {
            Console.Write("Recipe ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out var rid)) return;
            var recipe = await _dbContext.Recipes.FindAsync(rid);
            if (recipe == null)
            {
                Console.WriteLine("Recipe not found.");
                return;
            }

            Console.Write($"New title (current: {recipe.Title}): ");
            var t = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(t)) recipe.Title = t.Trim();

            Console.Write($"New description (current: {recipe.Description}): ");
            var d = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(d)) recipe.Description = d.Trim();

            Console.Write($"New instructions (current: {recipe.Instructions}): ");
            var ins = Console.ReadLine(); if (!string.IsNullOrWhiteSpace(ins)) recipe.Instructions = ins.Trim();

            Console.Write($"New prep time (current: {recipe.Preptime}): ");
            if (int.TryParse(Console.ReadLine(), out var pNew)) recipe.Preptime = pNew;

            Console.Write($"New cook time (current: {recipe.Cooktime}): ");
            if (int.TryParse(Console.ReadLine(), out var cNew)) recipe.Cooktime = cNew;

            Console.Write($"New serving size (current: {recipe.Servingsize}): ");
            if (int.TryParse(Console.ReadLine(), out var sNew)) recipe.Servingsize = sNew;

            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Recipe updated.");
        }

        public async Task DeleteRecipe()
        {
            Console.Write("Recipe ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out var rid)) return;
            var recipe = await _dbContext.Recipes.FindAsync(rid);
            if (recipe == null)
            {
                Console.WriteLine("Recipe not found.");
                return;
            }

            _dbContext.Recipes.Remove(recipe);
            await _dbContext.SaveChangesAsync();
            Console.WriteLine("Recipe deleted.");
        }

        public async Task FetchRecipeByCategory()
        {
            Console.Write("Category name: ");
            var cat = Console.ReadLine()?.Trim() ?? string.Empty;
            var lowerCat = cat.ToLowerInvariant();

            var list = await _dbContext.Recipes
                .Where(r => r.Categories.Any(c => c.Name.ToLower() == lowerCat))
                .ToListAsync();

            Console.WriteLine($"\nRecipes in '{cat}':");
            list.ForEach(r => Console.WriteLine($"  [{r.Recipeid}] {r.Title}"));
        }

        public async Task SearchRecipeByIngredients()
        {
            Console.Write("Enter ingredient names (comma-separated): ");
            var inputs = Console.ReadLine()?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList() ?? new List<string>();

            var query = _dbContext.Recipes.AsQueryable();
            foreach (var ing in inputs)
            {
                var lowerIng = ing.ToLowerInvariant();
                query = query.Where(r => r.Recipeingredients.Any(ri => ri.Ingredient.Name.ToLower() == lowerIng));
            }

            var recipes = await query.ToListAsync();
            Console.WriteLine($"\nRecipes with [{string.Join(", ", inputs)}]:");
            recipes.ForEach(r => Console.WriteLine($"  [{r.Recipeid}] {r.Title}"));
        }
    }
}

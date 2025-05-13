using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeDatabaseApp.Controllers;
using RecipeDatabaseApp.Entities;

namespace RecipeDatabaseApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Build DbContextOptions for Npgsql provider
            var optionsBuilder = new DbContextOptionsBuilder<RecipeDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RecipeDB;Username=postgres;Password=yourpass");

            // Create the DbContext
            using var dbContext = new RecipeDbContext(optionsBuilder.Options);

            // Run the interactive menu
            await RunMenu(dbContext);
        }

        /// <summary>
        /// Generates a terminal menu to interact with the recipe database.
        /// </summary>
        private static async Task RunMenu(RecipeDbContext dbContext)
        {
            var recipeController = new RecipeController(dbContext);
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("\n=== Recipe Database App ===");
                Console.WriteLine("1. List All Recipes");
                Console.WriteLine("2. Add New Ingredient");
                Console.WriteLine("3. Add New Recipe");
                Console.WriteLine("4. Update Recipe");
                Console.WriteLine("5. Delete Recipe");
                Console.WriteLine("6. Fetch Recipes by Category");
                Console.WriteLine("7. Search Recipes by Ingredients");
                Console.WriteLine("8. Add Category to Recipe");
                Console.WriteLine("9. Remove Category from Recipe");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                var input = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (input)
                    {
                        case "1": await recipeController.ListAllRecipes(); break;
                        case "2": await recipeController.AddNewIngredient(); break;
                        case "3": await recipeController.AddNewRecipe(); break;
                        case "4": await recipeController.UpdateRecipe(); break;
                        case "5": await recipeController.DeleteRecipe(); break;
                        case "6": await recipeController.FetchRecipeByCategory(); break;
                        case "7": await recipeController.SearchRecipeByIngredients(); break;   // <-- here
                        case "8": await recipeController.AddCategoryToRecipe(); break;         // <-- and here
                     //3   case "9": await recipeController.RemoveCategoryFromRecipe(); break;
                        case "0": exit = true; break;
                        default: Console.WriteLine("Invalid selection. Try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("================================\n");
            }
        }
    }
}
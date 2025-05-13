using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RecipeDatabaseApp.Entities;

public partial class RecipeDbContext : DbContext
{
    public RecipeDbContext()
    {
    }

    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<Recipeingredient> Recipeingredients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=RecipeDB;Username=postgres;Password=yourpass");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("category_pkey");

            entity.ToTable("category");

            entity.HasIndex(e => e.Name, "category_name_key").IsUnique();

            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Ingredientid).HasName("ingredient_pkey");

            entity.ToTable("ingredient");

            entity.HasIndex(e => e.Name, "ingredient_name_key").IsUnique();

            entity.Property(e => e.Ingredientid).HasColumnName("ingredientid");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Unit)
                .HasMaxLength(20)
                .HasColumnName("unit");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Recipeid).HasName("recipe_pkey");

            entity.ToTable("recipe");

            entity.HasIndex(e => e.Title, "recipe_title_key").IsUnique();

            entity.Property(e => e.Recipeid).HasColumnName("recipeid");
            entity.Property(e => e.Cooktime).HasColumnName("cooktime");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.Preptime).HasColumnName("preptime");
            entity.Property(e => e.Servingsize).HasColumnName("servingsize");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasMany(d => d.Categories).WithMany(p => p.Recipes)
                .UsingEntity<Dictionary<string, object>>(
                    "Recipecategory",
                    r => r.HasOne<Category>().WithMany()
                        .HasForeignKey("Categoryid")
                        .HasConstraintName("recipecategory_categoryid_fkey"),
                    l => l.HasOne<Recipe>().WithMany()
                        .HasForeignKey("Recipeid")
                        .HasConstraintName("recipecategory_recipeid_fkey"),
                    j =>
                    {
                        j.HasKey("Recipeid", "Categoryid").HasName("recipecategory_pkey");
                        j.ToTable("recipecategory");
                        j.IndexerProperty<int>("Recipeid").HasColumnName("recipeid");
                        j.IndexerProperty<int>("Categoryid").HasColumnName("categoryid");
                    });
        });

        modelBuilder.Entity<Recipeingredient>(entity =>
        {
            entity.HasKey(e => new { e.Recipeid, e.Ingredientid }).HasName("recipeingredient_pkey");

            entity.ToTable("recipeingredient");

            entity.Property(e => e.Recipeid).HasColumnName("recipeid");
            entity.Property(e => e.Ingredientid).HasColumnName("ingredientid");
            entity.Property(e => e.Quantity)
                .HasPrecision(8, 2)
                .HasColumnName("quantity");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.Recipeingredients)
                .HasForeignKey(d => d.Ingredientid)
                .HasConstraintName("recipeingredient_ingredientid_fkey");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Recipeingredients)
                .HasForeignKey(d => d.Recipeid)
                .HasConstraintName("recipeingredient_recipeid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

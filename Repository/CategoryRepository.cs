﻿using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(x => x.Id == id);
        }

        public bool CreateCategory(Category category)
        {
            _context.Add(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _context.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _context.Categories.Where(x => x.Id == id).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonByCategory(int categoryId)
        {
           return _context.PokemonCategories.Where(x=>x.CategoryId == categoryId).Select(x => x.Pokemon).ToList();
        }

        public bool Save()
        {
            var saved =  _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}

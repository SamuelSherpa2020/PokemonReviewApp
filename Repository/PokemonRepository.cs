﻿using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository: IPokemonRepository
    {
        private readonly ApplicationDbContext _context;
        public PokemonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.Owners.Where(a => a.Id == ownerId).FirstOrDefault();
            var category = _context.Categories.Where(a => a.Id == categoryId).FirstOrDefault();

            var pokemonOwner = new PokemonOwner()
            {
                Owner = pokemonOwnerEntity,
                Pokemon = pokemon,
            };

            _context.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon,
            };
            _context.Add(pokemonCategory);

            _context.Add(pokemon);
            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);
            return Save();
        }

        public Pokemon GetPokemon(int id)
        {

            return _context.Pokemons
                .Include(p => p.PokemonCategories)
                .ThenInclude(pc => pc.Category) // Ensure Category is loaded
                .FirstOrDefault(p => p.Id == id);
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokeId)
        {
            var review = _context.Reviews.Where(p => p.Pokemon.Id == pokeId);
            if(review.Count() <= 0)
            {
                return 0;
            }
            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons
                .Include(p => p.PokemonCategories)
                .ThenInclude(pc => pc.Category) 
                .OrderBy(p=>p.Id)
                .ToList();
        }

        public bool PokemonExists(int pokeId)
        {
            return _context.Pokemons.Any(x => x.Id == pokeId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var pokemonOwnerEntity = _context.PokemonOwners.
                Where(o => o.PokemonId == pokemon.Id).FirstOrDefault();

            var OwnerEntity = _context.Owners.
                Where(o => o.Id == ownerId).FirstOrDefault();

            var pokemonCategoryEntity = _context.PokemonCategories.
                Where(c => c.PokemonId == pokemon.Id).FirstOrDefault();

            var categoryEntity = _context.Categories.Where(o => o.Id == categoryId).FirstOrDefault();

            if (pokemonOwnerEntity != null && OwnerEntity != null)
            {

                _context.Remove(pokemonOwnerEntity);

                var pokemonOwner = new PokemonOwner()
                {
                    Owner = OwnerEntity,
                    Pokemon = pokemon,
                };
                _context.Add(pokemonOwner);
            }

            if (pokemonCategoryEntity != null && categoryEntity != null)
            {

                _context.Remove(pokemonCategoryEntity);

                var pokemonCategory = new PokemonCategory()
                {
                    Category = categoryEntity,
                    Pokemon = pokemon,
                };
                _context.Add(pokemonCategory);
            }
            else
                return false;


            _context.Update(pokemon);
            return Save();
        }
    }
}

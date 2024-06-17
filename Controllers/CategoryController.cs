using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        public IActionResult GetCategories()
        {
            var categories = _mapper.Map<List<CategoryDTO>>(_categoryRepository.GetCategories());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }

        #region You Can UnComment Your Previous Code
        //[HttpGet("{categoryId}")]
        //[ProducesResponseType(200, Type = typeof(Category))]
        //public IActionResult GetCategory(int categoryId)
        //{
        //    if (!_categoryRepository.CategoryExists(categoryId))
        //    {
        //        return NotFound();
        //    }
        //    var category = _mapper.Map<CategoryDTO>(_categoryRepository.GetCategory(categoryId));
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    return Ok(category);
        //}
        #endregion

        //[HttpGet("{categoryId}")]
        //[ProducesResponseType(200, Type = typeof(CategoryDTO))]
        //public async Task<IActionResult> GetCategory(int categoryId)
        //{
        //    if (!_categoryRepository.CategoryExists(categoryId))
        //    {
        //        return NotFound();
        //    }

        //    var category = await _categoryRepository.GetCategoryAsync(categoryId);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    var categoryDTO = _mapper.Map<CategoryDTO>(category);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    return Ok(categoryDTO);
        //}

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(ResponseModel<CategoryDTO>))]
        [ProducesResponseType(404, Type = typeof(ResponseModel<string>))]
        [ProducesResponseType(400, Type = typeof(ResponseModel<string>))]
        [ProducesResponseType(500, Type = typeof(ResponseModel<string>))]
        public async Task<ActionResult<ResponseModel<CategoryDTO>>> GetCategory(int categoryId)
        {
            try
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return NotFound(new ResponseModel<string>(false, "No Category of Such Id Exists !", null!));
                }

                var category = await _categoryRepository.GetCategoryAsync(categoryId);
                if (category == null)
                {
                    return NotFound(new ResponseModel<string>(false, "Category Could not be loaded from DB.", null!));
                }

                var categoryDTO = _mapper.Map<CategoryDTO>(category);
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseModel<string>(false, "Category Mapping Failed", null!));
                }

                return Ok(new ResponseModel<CategoryDTO>(true, "Category retrieved successfully", categoryDTO));
            }
            catch (AutoMapperMappingException ex)
            {
                return BadRequest(new ResponseModel<string>(false, $"An error occurred while mapping the category data:-{ex.Message}", null!));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel<string>(false, $"An unexpected error occurred:- {ex.Message}", null!));
            }
        }


        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(
                _categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult CreateCategory([FromBody] CategoryDTO categoryCreate)
        {
            if (categoryCreate == null)
            {
                return BadRequest(ModelState);
            }

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryCreate);
            if (!_categoryRepository.CreateCategory(categoryMap)) {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDTO updatedCategory) {
            if (updatedCategory == null)
            {
                return BadRequest(ModelState);
            }
            if (categoryId != updatedCategory.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var categoryMap = _mapper.Map<Category>(updatedCategory);
            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
             
            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
            }

            return NoContent();
        }

    }
}

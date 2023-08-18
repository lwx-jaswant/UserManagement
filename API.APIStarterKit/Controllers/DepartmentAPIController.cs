using API.APIStarterKit.GenericRepo;
using Core.Data.Models;
using Core.Data.Models.CommonViewModel;
using Core.Data.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.APIStarterKit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentAPIController : ControllerBase
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentAPIController(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        [HttpGet]
        [Authorize(Roles = MainMenu.Department.RoleName)]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
        {
            var result = await _departmentRepository.GetAllAsync();
            return Ok(result);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<Department>> GetById(Int64 id)
        {
            return Ok(await _departmentRepository.GetByIdAsync(id));
        }
        [HttpGet]
        [Route("GetByName/{DepartmentName}")]
        public async Task<ActionResult<Department>> GetByName(string DepartmentName)
        {
            return Ok(await _departmentRepository.FindByConditionAsync(x => x.Name == DepartmentName));
        }
        [HttpPost]
        [Route("AddEdit")]
        public async Task<IActionResult> AddEdit([FromBody] Department _Department)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            var _UserName = HttpContext.User.Identity.Name;
            try
            {
                if (_Department.Id > 0)
                {
                    var currentEmployeeType = await _departmentRepository.GetByIdAsync(_Department.Id);
                    _Department.CreatedDate = currentEmployeeType.CreatedDate;
                    _Department.CreatedBy = currentEmployeeType.CreatedBy;
                    _Department.ModifiedBy = _UserName;
                    _departmentRepository.Update(_Department, currentEmployeeType);
                    
                    _JsonResultViewModel.IsSuccess = await _departmentRepository.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Department has been updated successfully. Department Id: " + _Department.Id;
                    return Ok(_JsonResultViewModel);
                }
                else
                {
                    _Department.CreatedBy = _UserName;
                    _Department.ModifiedBy = _UserName;
                    _departmentRepository.Add(_Department);

                    _JsonResultViewModel.IsSuccess = await _departmentRepository.SaveChangesAsync();
                    _JsonResultViewModel.AlertMessage = "Department has been created successfully. Department Id: " + _Department.Id;
                    return Ok(_JsonResultViewModel);
                }
            }
            catch (Exception ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                _JsonResultViewModel.AlertMessage = ex.Message;
                return Ok(_JsonResultViewModel);
                throw ex;
            }
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(Int64 id)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                await _departmentRepository.Delete(id);
                _JsonResultViewModel.IsSuccess = await _departmentRepository.SaveChangesAsync();
                _JsonResultViewModel.AlertMessage = "Department has been deleted successfully. Department Id: " + id;
                _JsonResultViewModel.IsSuccess = true;
                return Ok(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                _JsonResultViewModel.AlertMessage = ex.Message;
                return Ok(_JsonResultViewModel);
                throw ex;
            }
        }
    }
}

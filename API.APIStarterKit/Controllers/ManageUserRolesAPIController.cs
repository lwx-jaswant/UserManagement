using API.APIStarterKit.GenericRepo;
using API.APIStarterKit.Services;
using Core.Data.Context;
using Core.Data.Models;
using Core.Data.Models.CommonViewModel;
using Core.Data.Models.ManageUserRolesVM;
using Core.Data.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.APIStarterKit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ManageUserRolesAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<ManageUserRoles> _Repository;
        private readonly ICommonService _iCommonService;
        private readonly IRolesService _iRolesService;
        public ManageUserRolesAPIController(ApplicationDbContext context, IRepository<ManageUserRoles> repository, ICommonService iCommonService, IRolesService iRolesService)
        {
            _context = context;
            _Repository = repository;
            _iCommonService = iCommonService;
            _iRolesService = iRolesService;
        }
        [HttpGet]
        [Authorize(Roles = MainMenu.ManageUserRoles.RoleName)]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<ManageUserRoles>>> GetAll()
        {
            var result = await _Repository.GetAllAsync();
            return Ok(result);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ActionResult<ManageUserRoles>> GetById(Int64 id)
        {
            return Ok(await _Repository.GetByIdAsync(id));
        }
        [HttpGet]
        [Route("GetByIdDetails/{id}")]
        public async Task<ActionResult<List<ManageUserRolesViewModel>>> GetByIdDetails(Int64 id)
        {
            ManageUserRolesCRUDViewModel vm = new();
            if (id > 0)
            {
                vm = await _Repository.GetByIdAsync(id);
                vm.listManageUserRolesViewModel = await _iCommonService.GetManageRoleDetailsList(id);
            }
            else
            {
                vm.listManageUserRolesViewModel = await _iRolesService.GetRoleList();
            }
            return Ok(vm);
        }
        [HttpPost]
        [Route("AddEdit")]
        public async Task<IActionResult> AddEdit([FromBody] ManageUserRolesCRUDViewModel model)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            ManageUserRoles _ManageUserRoles = new();
            var _UserName = HttpContext.User.Identity.Name;
            try
            {
                if (model.Id > 0)
                {
                    var currentManageUserRoles = await _Repository.GetByIdAsync(model.Id);
                    model.CreatedDate = _ManageUserRoles.CreatedDate;
                    model.CreatedBy = _ManageUserRoles.CreatedBy;
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = _UserName;
                    _Repository.Update(model, currentManageUserRoles);
                    await _Repository.SaveChangesAsync();

                    foreach (var item in model.listManageUserRolesViewModel)
                    {
                        var _ManageUserRolesDetails = await _context.ManageUserRolesDetails.FindAsync(item.ManageRoleDetailsId);
                        _ManageUserRolesDetails.IsAllowed = item.IsAllowed;
                        _context.ManageUserRolesDetails.Update(_ManageUserRolesDetails);
                        await _Repository.SaveChangesAsync();
                    }

                    _JsonResultViewModel.AlertMessage = "Manage User Roles has been updated successfully. Manage User Roles Id: " + model.Id;
                    _JsonResultViewModel.IsSuccess = true;
                    return Ok(_JsonResultViewModel);
                }
                else
                {
                    _ManageUserRoles = model;
                    _ManageUserRoles.CreatedDate = DateTime.Now;
                    _ManageUserRoles.ModifiedDate = DateTime.Now;
                    _ManageUserRoles.CreatedBy = _UserName;
                    _ManageUserRoles.ModifiedBy = _UserName;
                    _Repository.Add(_ManageUserRoles);
                    await _Repository.SaveChangesAsync();

                    foreach (var item in model.listManageUserRolesViewModel)
                    {
                        ManageUserRolesDetails _ManageRoleDetails = new();
                        _ManageRoleDetails.ManageRoleId = _ManageUserRoles.Id;
                        _ManageRoleDetails.RoleId = item.RoleId;
                        _ManageRoleDetails.RoleName = item.RoleName;
                        _ManageRoleDetails.IsAllowed = item.IsAllowed;

                        _ManageRoleDetails.CreatedDate = DateTime.Now;
                        _ManageRoleDetails.ModifiedDate = DateTime.Now;
                        _ManageRoleDetails.CreatedBy = _UserName;
                        _ManageRoleDetails.ModifiedBy = _UserName;
                        _context.Add(_ManageRoleDetails);
                        await _context.SaveChangesAsync();
                    }

                    _JsonResultViewModel.IsSuccess = true;
                    _JsonResultViewModel.AlertMessage = "Manage User Roles has been created successfully. Manage User Roles Id: " + _ManageUserRoles.Id;
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
                await _Repository.Delete(id);
                _JsonResultViewModel.IsSuccess = await _Repository.SaveChangesAsync();
                _JsonResultViewModel.AlertMessage = "Manage User Roles has been deleted successfully. Manage User Roles Id: " + id;
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

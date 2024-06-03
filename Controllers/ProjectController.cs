using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Glimpse.Models;
using Glimpse.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Glimpse.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly GlimpseContext _db;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly UserManager<User> _userManager;

    public ProjectController(GlimpseContext db, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
    {
        _db = db;
        _hostEnvironment = hostEnvironment;
        _userManager = userManager;
    }

    public async Task<IActionResult> MainProjects()
    {
        string userId = _userManager.GetUserId(User);

        var user = _db.Users
            .Include(u => u.Projects)
            .Single(u => u.Id == userId);

        var userProjects = user.Projects;
        ICollection<Project> activeUserProjects = [];
        foreach(Project project in userProjects)
        {
            if(project.IsActive)
            {
                activeUserProjects.Add(project);
            }
        }
        ViewData["UserName"] = user.FirstName + " " + user.LastName;

        return View(activeUserProjects);
    }

    // CREATE
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Project project, IFormFile projectImg)
    {
        project.CreationDate = DateOnly.FromDateTime(DateTime.UtcNow);
        project.LastEdited = DateTime.UtcNow;
        project.IsActive = true;
        project.ResponsibleUserId = _userManager.GetUserId(User);

        var currentUser = await _userManager.GetUserAsync(User);

        project.Users.Add(currentUser);

        if (ModelState.IsValid)
        {
            if (projectImg != null && projectImg.Length > 0)
            {
                string pastaUploads = Path.Combine(_hostEnvironment.WebRootPath, "project-pictures");
                string nomeArquivo = Guid.NewGuid() + "_" + Path.GetFileName(projectImg.FileName);
                string caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);
                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await projectImg.CopyToAsync(stream);
                }
                project.Picture = "../project-pictures/" + nomeArquivo;
            }
            _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();

            var DefaultBoard = new Board 
            {
                Name = "Meu_Quadro",
                CreationDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsActive = true,
                CreatorId = currentUser.Id,
                Project = project,
            };
            var BacklogLane = new Lane
            {
                Name = "BackLog",
                Board = DefaultBoard,
                Index = 0,
            };
            var ToDoLane = new Lane
            {
                Name = "To Do",
                Board = DefaultBoard,
                Index = 1,
            };
            var DoneLane = new Lane
            {
                Name = "Done",
                Board = DefaultBoard,
                Index = 2,
            };
            DefaultBoard.Lanes.Add(BacklogLane);
            DefaultBoard.Lanes.Add(ToDoLane);
            DefaultBoard.Lanes.Add(DoneLane);
            project.Boards.Add(DefaultBoard);

            var DefaultPORole = new Role
            {
                Name = "Product Owner",  // Assign a default role name or customize as needed
                Description = "This is the default Developer role created during project creation",
                Color = "#74B72E",  // Assign a default color or customize as needed
                CanManageMembers = true,
                CanManageRoles = true,
                CanManageCards = true,
                CanManageTags = true,
                CanManageChecklist = true,
                Project = project
            };
            var DefaultDevTeamRole = new Role
            {
                Name = "Developer",  // Assign a default role name or customize as needed
                Description = "This is the default Developer role created during project creation",
                Color = "#FF1D8E",  // Assign a default color or customize as needed
                CanManageMembers = false,
                CanManageRoles = true,
                CanManageCards = true,
                CanManageTags = true,
                CanManageChecklist = true,
                Project = project
            };
            project.Roles.Add(DefaultPORole);
            project.Roles.Add(DefaultDevTeamRole);
            var user = await _db.Users
                .Include(u => u.Projects)
                .SingleAsync(u => u.Id == project.ResponsibleUserId);
            user.Roles.Add(DefaultPORole);
            user.Projects.Add(project);
            await _db.SaveChangesAsync();

            return RedirectToAction("GetBoardInfo", "Board", new { id = DefaultBoard.Id });
        }

        return View("Create", project);
    }

    public async Task<IActionResult> Edit(int id)
    {
        Project Project = await _db.Projects.FindAsync(id);

        if (Project == null)
        {
            return NotFound();
        }
        return View(Project);
    }

    [HttpPost]
    public async Task<IActionResult> EditProject(Project Project, IFormFile projectImg)
    {
        if (ModelState.IsValid)
        {
            if (projectImg != null && projectImg.Length > 0)
            {
                string pastaUploads = Path.Combine(_hostEnvironment.WebRootPath, "project-pictures");
                string nomeArquivo = Guid.NewGuid() + "_" + Path.GetFileName(projectImg.FileName);
                string caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);
                using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
                {
                    await projectImg.CopyToAsync(stream);
                }
                Project.Picture = "../project-pictures/" + nomeArquivo;
            }
            Project oldProject = await _db.Projects.FindAsync(Project.Id);

            _db.Entry(oldProject).CurrentValues.SetValues(Project);
            await _db.SaveChangesAsync();

            return RedirectToAction("MainProjects");
        }

        return View("Edit", Project);
    }

    public async Task<IActionResult> Delete(int id)
    {
        Project Project = await _db.Projects.FindAsync(id);

        if (Project == null)
        {
            return NotFound();
        }

        return View(Project);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProject(Project project)
    {
        if (ModelState.IsValid)
        {
            project.IsActive = false;
            Project currentProject = await _db.Projects.FindAsync(project.Id);
            _db.Entry(currentProject).CurrentValues.SetValues(project);
            await _db.SaveChangesAsync();

            return RedirectToAction("MainProjects");
        }

        return View("Delete", project);
    }

    public async Task<IActionResult> Users(int projectId)
    {
        Project project = _db.Projects
            .Include(p => p.Users)
                .ThenInclude(u => u.Roles)
            .Include(p => p.Roles.Where(r => r.Project.Id == projectId))
            .Single(p => p.Id == projectId);

        if (project == null)
        {
            return NotFound();
        }

        ICollection<User> users = [];
        User toAddUser = new();
        foreach (User user in project.Users)
        {
            toAddUser = user;
            // toAddUser.Roles = user.Roles.Where(r => project.Roles.Any(pr => pr.Id == r.Id)).ToList();
            users.Add(toAddUser);
        }
        Project viewProject = project;
        viewProject.Users = users;

        return View(viewProject);
    }

    public async Task<IActionResult> AddUser(int projectId, string userEmail)
    {
        Project project = await _db.Projects.FindAsync(projectId);
        if (project == null)
        {
            return NotFound("Projeto não existe");
        }
        var user = await _userManager.FindByNameAsync(userEmail);
        if (project == null)
        {
            return NotFound("Email digitado não pertence a nenhum usuário");
        }
        project.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToAction("Users", new { projectId });
    }

}

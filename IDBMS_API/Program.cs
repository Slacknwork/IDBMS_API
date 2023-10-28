using API.Services;
using API.Supporters;
using API.Supporters.JwtAuthSupport;
using BLL.Services;
using BusinessObject.Models;
using IDBMS_API.Services;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Repository.Implements;
using Repository.Interfaces;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAuthenticationCodeRepository, AuthenticationCodeRepository>();
builder.Services.AddScoped<IApplianceSuggestionRepository, ApplianceSuggestionRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IConstructionTaskCategoryRepository, ConstructionTaskCategoryRepository>();
builder.Services.AddScoped<IConstructionTaskRepository, ConstructionTaskRepository>(); 
builder.Services.AddScoped<IConstructionTaskDesignRepository, ConstructionTaskDesignRepository>();
builder.Services.AddScoped<IConstructionTaskReportRepository, ConstructionTaskReportRepository>();
builder.Services.AddScoped<IDecorProgressReportRepository, DecorProgressReportRepository>();
builder.Services.AddScoped<IDecorProjectDesignRepository, DecorProjectDesignRepository>();
builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IInteriorItemBookmarkRepository, InteriorItemBookmarkRepository>();
builder.Services.AddScoped<IInteriorItemCategoryRepository, InteriorItemCategoryRepository>();
builder.Services.AddScoped<IInteriorItemColorRepository, InteriorItemColorRepository>();
builder.Services.AddScoped<IInteriorItemRepository, InteriorItemRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IParticipationRepository, ParticipationRepository>();
builder.Services.AddScoped<IPrepayStageRepository, PrepayStageRepository>();
builder.Services.AddScoped<IPrepayStageDesignRepository, PrepayStageDesignRepository>();
builder.Services.AddScoped<IProjectCategoryRepository, ProjectCategoryRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectDocumentRepository, ProjectDocumentRepository>();
builder.Services.AddScoped<IProjectDocumentTemplateRepository, DocumentTemplateRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<AdminService, AdminService>();
builder.Services.AddScoped<AuthenticationCodeService, AuthenticationCodeService>();
builder.Services.AddScoped<ApplianceSuggestionService, ApplianceSuggestionService>();
builder.Services.AddScoped<CommentService, CommentService>();
builder.Services.AddScoped<ConstructionTaskCategoryService, ConstructionTaskCategoryService>();
builder.Services.AddScoped<ConstructionTaskService, ConstructionTaskService>();
builder.Services.AddScoped<ConstructionTaskDesignService, ConstructionTaskDesignService>();
builder.Services.AddScoped<ConstructionTaskReportService, ConstructionTaskReportService>();
builder.Services.AddScoped<DecorProgressReportService, DecorProgressReportService>();
builder.Services.AddScoped<DecorProjectDesignService, DecorProjectDesignService>();
builder.Services.AddScoped<FloorService, FloorService>();
builder.Services.AddScoped<InteriorItemBookmarkService, InteriorItemBookmarkService>();
builder.Services.AddScoped<InteriorItemCategoryService, InteriorItemCategoryService>();
builder.Services.AddScoped<InteriorItemColorService, InteriorItemColorService>();
builder.Services.AddScoped<InteriorItemService, InteriorItemService>();
builder.Services.AddScoped<NotificationService, NotificationService>();
builder.Services.AddScoped<ParticipationService, ParticipationService>();
builder.Services.AddScoped<PrepayStageService, PrepayStageService>();
builder.Services.AddScoped<PrepayStageDesignService, PrepayStageDesignService>();
builder.Services.AddScoped<ProjectCategoryService, ProjectCategoryService>();
builder.Services.AddScoped<ProjectService, ProjectService>();
builder.Services.AddScoped<ProjectDocumentService, ProjectDocumentService>();
builder.Services.AddScoped<DocumentTemplateService, DocumentTemplateService>();
builder.Services.AddScoped<RoomService, RoomService>();
builder.Services.AddScoped<RoomTypeService, RoomTypeService>();
builder.Services.AddScoped<TransactionService, TransactionService>();
builder.Services.AddScoped<UserService, UserService>();

builder.Services.AddScoped<FirebaseService, FirebaseService>();
builder.Services.AddScoped<JwtTokenSupporter, JwtTokenSupporter>();
builder.Services.AddControllers().AddJsonOptions(x =>
                    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddOData(option => option.Select().Filter()
                .Count().OrderBy().Expand().SetMaxTop(100).AddRouteComponents("odata", GetEdmModel()));
builder.Services.AddODataQueryFilter();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseODataBatching();

app.UseMiddleware<JWTMiddleware>();

app.MapControllers();

app.Run();


static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

    builder.EntitySet<Admin>("Admins");
    builder.EntitySet<ApplianceSuggestion>("ApplianceSuggestions");
    builder.EntitySet<AuthenticationCode>("AuthenticationCodes");
    builder.EntitySet<Comment>("Comments"); 
    builder.EntitySet<ConstructionTask>("ConstructionTasks");
    builder.EntitySet<ConstructionTaskCategory>("ConstructionTaskCategories"); 
    builder.EntitySet<ConstructionTaskDesign>("ConstructionTaskDesigns");
    builder.EntitySet<ConstructionTaskReport>("ConstructionTaskReports"); 
    builder.EntitySet<DecorProgressReport>("DecorProgressReports");
    builder.EntitySet<DecorProjectDesign>("DecorProjectDesigns"); 
    builder.EntitySet<Floor>("Floors");
    builder.EntitySet<InteriorItem>("InteriorItems");
    builder.EntitySet<InteriorItemBookmark>("InteriorItemBookmarks");
    builder.EntitySet<InteriorItemCategory>("InteriorItemCategories");
    builder.EntitySet<InteriorItemColor>("InteriorItemColors");
    builder.EntitySet<Notification>("Notifications");
    builder.EntitySet<Participation>("Participations");
    builder.EntitySet<PrepayStage>("PrepayStages");
    builder.EntitySet<PrepayStageDesign>("PrepayStageDesigns");
    builder.EntitySet<Project>("Projects");
    builder.EntitySet<ProjectCategory>("ProjectCategories");
    builder.EntitySet<ProjectDocument>("ProjectDocuments");
    builder.EntitySet<ProjectDocumentTemplate>("DocumentTemplates");
    builder.EntitySet<Room>("Rooms");
    builder.EntitySet<RoomType>("RoomTypes");
    builder.EntitySet<Transaction>("Transactions");
    builder.EntitySet<User>("Users");
    builder.EntitySet<UserRole>("UserRoles");

    return builder.GetEdmModel();
}
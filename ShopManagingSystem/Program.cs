using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SMS_DataAccess.Data;
using SMS_DataAccess.Repository;
using SMS_DataAccess.Repository.IRepository;
using SMS_Utility.BrainTreePayment;
using SMS_Utility.BrainTreePayment.Interface;
using SMS_Utility.EmailSending;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddHttpContextAccessor()
    .AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    })
    .AddControllersWithViews();

builder.Services.Configure<BrainTreeSettings>(builder.Configuration.GetSection("BrainTree"));
builder.Services.AddSingleton<IBrainTreeGate, BrainTreeGate>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IApplicationTypeRepository, ApplicationTypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
builder.Services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();
builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();


builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

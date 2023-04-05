using ChatBox;
using ChatBox.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(p =>
{
    p.ExpireTimeSpan = TimeSpan.FromDays(32);
    p.LoginPath = "/auth/signin";
    p.LogoutPath = "/auth/signout";

});
//builder.Services.AddMvc();
builder.Services.AddTransient<ContactFilter>();
builder.Services.AddTransient<IDbConnection, SqlConnection>(p => new SqlConnection(builder.Configuration.GetConnectionString("ChatBox")));
builder.Services.AddTransient<IMemberRepository, MemberRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IConversationRepository, ConversationRepository>();


builder.Services.AddSignalR();
builder.Services.AddMvc();
var app = builder.Build();
app.MapHub<ChatHub>("/chathub");

//app.MapGet("/", () => "Hello World!");
app.UseStaticFiles(); //? what is it use for cause if dont have it cant load any bootstrap
//app.MapDefaultControllerRoute(); //idk but if dont have this cant show any view
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=home}/{action=signin}"
//    );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Signin}");

app.Run();

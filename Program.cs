using ChatIADesktop.Components;
using ChatIADesktop.Servicos;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registra o HttpClient
builder.Services.AddHttpClient();

// Adiciona o cache de memória para o ServicoExemplos
builder.Services.AddMemoryCache();

// Registra os serviços da aplicação
builder.Services.AddScoped<IServicoOllama, ServicoOllama>();
builder.Services.AddScoped<IProcessadorArquivos, ProcessadorArquivos>();
builder.Services.AddScoped<IAnalisadorSentimento, AnalisadorSentimento>();
builder.Services.AddScoped<IExplicadorTermos, ExplicadorTermos>();
builder.Services.AddScoped<IServicoExemplos, ServicoExemplos>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}
else
{
    // No HTTPS redirection in development to make local testing easier
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
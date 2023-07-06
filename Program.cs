using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearRegression;
using Microsoft.AspNetCore.Components.Forms;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapPost("/api/calculateSlope", async context =>
{
    var data = await context.Request.ReadFromJsonAsync<Dictionary<int, double>>();
    List<int> estatura = new List<int>(data.Keys);
    List<double> promedio = new List<double>(data.Values);
    (double pendiente, double interceptor) = CalculateSlope(estatura, promedio);
    var result = new { Pendiente = pendiente, Interceptor = interceptor };
    await context.Response.WriteAsJsonAsync(result);
});

(double pendiente, double interceptor) CalculateSlope(List<int> estatura, List<double> pr)
{
    double sumEstatura = 0, sumPr = 0, sumEstaturaPr = 0, sumEstaturaCuadrados = 0;

    for (int i = 0; i < estatura.Count; i++)
    {
        sumEstatura += estatura[i];
        sumPr += pr[i];
        sumEstaturaPr += pr[i] * estatura[i];
        sumEstaturaCuadrados += Math.Pow(estatura[i], 2);
    }

    int n = estatura.Count;
    double pendiente = (n * sumEstaturaPr - sumEstatura * sumPr) / (n * sumEstaturaCuadrados - Math.Pow(sumEstatura, 2));
    double interceptor = (sumPr - pendiente * sumEstatura) / n;

    return (pendiente, interceptor);
}

app.UseAuthorization();

app.MapRazorPages();

app.Run();

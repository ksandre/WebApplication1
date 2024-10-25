var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();
builder.Services.AddSingleton<RabbitMqListener>();
builder.Services.AddHostedService<RabbitMqListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// Add middleware in the correct order.
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(options => options.WithOrigins("http://localhost:3000").AllowAnyHeader().WithMethods("GET", "POST").AllowCredentials());
app.UseAuthorization();

// Define routes directly on the `app` object.
app.MapControllers();
app.MapHub<ChatHub>("/chat-hub");

app.Run();

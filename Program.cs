var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Time after which the server will close the connection if it does not hear from the client
    options.KeepAliveInterval = TimeSpan.FromMinutes(5);     // Keep-alive interval for server-to-client messages
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddSingleton<RabbitMqListener>();
builder.Services.AddHostedService<RabbitMqListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
//}

// Add middleware in the correct order.

//app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(options => options.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseAuthorization();

// Define routes directly on the `app` object.
app.MapControllers();
app.MapHub<ChatHub>("/Chat");

app.Run();

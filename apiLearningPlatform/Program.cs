using LearningClassLibary.Interfaces;
using MySql.Data.MySqlClient;
using LearningPlatform.API.Services;
using LearningClassLibrary.Interfaces;
using LearningClassLibrary.Services;


namespace apiLearningPlatform
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register the MySqlConnection
            builder.Services.AddScoped<MySqlConnection>(_ =>
                new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register the IPersonService with a connection string dependency
            builder.Services.AddScoped<IPersonService>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new PersonService(connectionString);
            });


            // Register the IQuestionService
            builder.Services.AddScoped<IQuestionService>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new QuestionService(connectionString);
            });

            // Register the ITopicService
            builder.Services.AddScoped<ITopicService>(provider =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                return new TopicService(connectionString);
            });




            Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

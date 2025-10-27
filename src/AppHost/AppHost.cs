using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var db = postgres.AddDatabase("db");

var api = builder.AddProject<Host>("api")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten(opt =>
{
    opt.Connection(builder.Configuration.GetConnectionString("Default"));
    //opt.Projections.Add<Projector>();
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Welcome page");

app.MapGet("/self-contained-projection",  (IQuerySession session) =>
{
    return  session.Events.AggregateStreamAsync<SelfContainedProjection>(new Guid("0188ab1c-fb6c-47fd-b4b0-bb26df222172"));
});

app.MapGet("/single-projection",  (IQuerySession session) =>
{
    return  session.Events.AggregateStreamAsync<SingleProjection>(new Guid("0188ab1c-fb6c-47fd-b4b0-bb26df222172"));
});

app.MapGet("/card", async (IDocumentSession session) =>
{
    var result = session.Events.StartStream(
        new addedToCard("T-shirt",1),
        new updatedShippingInformation("Ankara/Turkey","555-333-4444")
    );
    await session.SaveChangesAsync();
    return "created";
});
app.MapGet("/append", async (IDocumentSession session) =>
{
    session.Events.Append(
        new Guid("0188ab1c-fb6c-47fd-b4b0-bb26df222172"),
        new addedToCard("Sweatshirt - M",3)
    );
    await session.SaveChangesAsync();
    return "created";
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public class SelfContainedProjection
{
    public Guid Id { get; set; }
    public List<string> Products { get; set; } = new();

    public void Apply(addedToCard e)
    {
        Products.Add(e.name);
    }
}

public class SingleProjection
{
    public Guid Id { get; set; }
    public int TotalQuantity { get; set; }
}

public class Projector : SingleStreamProjection<SingleProjection>
{
    public Guid Id { get; set; }
    public List<string> Products { get; set; } = new();
    
    public void Apply(SingleProjection snapshot,addedToCard e)
    {
        snapshot.TotalQuantity += e.quantity;
    }
}
public record  addedToCard(string name,int quantity);
public record updatedShippingInformation(string address,string phoneNumber);
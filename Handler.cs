using Amazon.Lambda.Core;
using System.Collections;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
  public class Handler
  { 
    private string DB_HOST = System.Environment.GetEnvironmentVariable("DB_HOST");
    private string DB_NAME = System.Environment.GetEnvironmentVariable("DB_NAME");
    private string DB_USER = System.Environment.GetEnvironmentVariable("DB_USER");
    private string DB_PASSWORD = System.Environment.GetEnvironmentVariable("DB_PASSWORD"); 
    public APIGatewayProxyResponse GetCollections()
    {
      ArrayList collections = new ArrayList();
      Collection c1 = new Collection("1", "private", "Mary Brown");
      Collection c2 = new Collection("2", "private", "John Newman");
      Collection c3 = new Collection("3", "public", "");
      collections.Add(c1);
      collections.Add(c2);
      collections.Add(c3);
      string body = JsonSerializer.Serialize(collections);
      return new APIGatewayProxyResponse
      {
        // body must be a string, cannot be ArrayList
        Body = body,
        Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" }
            },
        StatusCode = 200,
      };
    }
    public ArrayList GetCollectionById(APIGatewayProxyRequest request)
    {
      string paramId = request.PathParameters["paramId"];
      // to see logger, run in terminal: serverless logs -f get-collection
      // LambdaLogger.Log("Param id = " +paramId);

// open up db connection
      MySqlConnection connection = new MySqlConnection($"server={DB_HOST};user id={DB_USER};password={DB_PASSWORD};port=3306;database={DB_NAME};");
      connection.Open();
// prepare mysql query
      var cmd = connection.CreateCommand();
      cmd.CommandText = @"SELECT * FROM `collections` WHERE `id` = @id";
      cmd.Parameters.AddWithValue("@id", paramId);
// execute the query
      MySqlDataReader reader = cmd.ExecuteReader();
      ArrayList collections = new ArrayList();
// loop through result
      while (reader.Read())
      {
        Collection collection = new Collection(reader.GetString("id"), reader.GetString("locationType"), reader.GetString("name"));
        collections.Add(collection);
      }
// close the connection
      connection.Close();
      return collections;
    }

    public string CreateCollection(APIGatewayProxyRequest request)
    {
      string body = request.Body;
      string paramId = request.PathParameters["paramId"];
      string msg = "Post request with param = " + paramId;
      return body;
    }
  }

  public class Collection
  {
    public string Id { get; set; }
    public string LocationType { get; set; }
    public string Name { get; set; }

    public Collection(string id, string locationType, string name)
    {
      Id = id;
      LocationType = locationType;
      Name = name;
    }
  }

}

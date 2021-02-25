using Amazon.Lambda.Core;
using System.Collections;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly:LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    {
       public APIGatewayProxyResponse GetCollections()
       {
          ArrayList collections = new ArrayList();
          Collection c1 = new Collection(1, "private", "Mary Brown");
          Collection c2 = new Collection(2, "private", "John Newman");
          Collection c3 = new Collection(3, "public", "");
          collections.Add(c1);
          collections.Add(c2);
          collections.Add(c3);
          string body = JsonSerializer.Serialize(collections);
          return new APIGatewayProxyResponse {
            // body must be a string, cannot be ArrayList
            Body = body,
            Headers = new Dictionary <string, string>
            { 
                { "Content-Type", "application/json" }, 
                { "Access-Control-Allow-Origin", "*" } 
            },
            StatusCode = 200,
          };
       }
       public string GetCollectionById(APIGatewayProxyRequest request)
       {
          string paramId = request.PathParameters["paramId"];
          // to see logger, run in terminal: serverless logs -f get-collection
          // LambdaLogger.Log("Param id = " +paramId);
          return paramId;
       }
    }

    public class Collection
    {
      public int Id {get;}
      public string LocationType {get;}
      public string Name {get;}

      public Collection(int id, string locationType, string name){
        Id = id;
        LocationType = locationType;
        Name = name;
      }
    }

    public class Request
    {
      public string Key1 {get; set;}
      public string Key2 {get; set;}
      public string Key3 {get; set;}
    }
}

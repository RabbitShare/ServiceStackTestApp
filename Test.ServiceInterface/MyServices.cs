using System;
using System.Linq;
using MongoDB.Driver;
using ServiceStack;
using Test.ServiceModel;
using Test.ServiceModel.Types;

namespace Test.ServiceInterface
{
    public class MyServices : Service
    {
        public IMongoDatabase MongoDatabase { get; set; }
        
        public PostResponse Post(Post request) => 
            new PostResponse { Result = request.Arg };
        
        public ActiveUsersGetResponse Get(ActiveUsersGet users)
        {
            var sessionKeys = Redis.SearchKeys("urn:iauthsession:*");
            return new ActiveUsersGetResponse
            {
                Users = Redis.GetValues<AuthUserSession>(sessionKeys)
                    .Select(s => s.UserName)
                    .ToArray()
            };
        }
        
        public TaskGetListResponse Get(TaskGetList tasks)
        {
            var userName = GetSession().UserName;
            var col = MongoDatabase.GetCollection<Task>("tasks");
            var activeTasks = col.Find(t => t.User == userName)
                .ToEnumerable()
                .ToArray();
            return new TaskGetListResponse { Tasks = activeTasks };
        }
        
        public void Post(TaskAdd task)
        {
            var session = GetSession();
            var col = MongoDatabase.GetCollection<Task>("tasks");
            col.InsertOne(new Task {User = session.UserName, Name = task.Name});
        }
    }
}

using System.Collections.Generic;
using ServiceStack;
using System;
using Test.ServiceModel.Types;

namespace Test.ServiceModel
{
    [Route("/post")]
    public class Post : IReturn<PostResponse>
    {
        public string Arg { get; set; }
    }

    public class PostResponse
    {
        public string Result { get; set; }
    }

    [Authenticate]
    [RequiredRole("Admin")]
    [Route("/active_users_get")]
    public class ActiveUsersGet : IReturn<ActiveUsersGetResponse>
    {
        
    }
    public class ActiveUsersGetResponse
    {
        public string[] Users { get; set; }
    }
    
    [Authenticate]
    [Route("/task_get_list")]
    public class TaskGetList : IReturn<TaskGetListResponse>
    {

    }

    public class TaskGetListResponse
    {
        public Task[] Tasks { get; set; }
    }

    [Authenticate]
    [Route("/task_add")]
    public class TaskAdd : IReturnVoid
    {
        public string Name { get; set; }
    }
}

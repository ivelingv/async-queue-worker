using System;
using System.Collections.Generic;
using System.Linq;

namespace PubSubExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var room = new Room { Name = "My cool room" };

            while(true)
            {
                var line = Console.ReadLine();
                var arguments = line.Split(' ');
                if (arguments[0] == "j")
                {
                    room.JoinUser(arguments[1]);
                }

                if (arguments[0] == "l")
                {
                    room.LeaveUser(arguments[1]);
                }

                if (arguments[0] == "list")
                {
                    room.PrintUsers();
                }

                if (arguments[0] == "clear")
                {
                    Console.Clear();
                }
            }
        }
    }

    public class UserJoiningEventArg : EventArgs
    {
        public string UserName { get; set; }

        public string RoomName { get; set; }
    }

    public class Room
    {
        protected event EventHandler<UserJoiningEventArg> OnUserJoining;
        protected event EventHandler<UserJoiningEventArg> OnUserLeaving;
        protected IList<User> Users { get; } = new List<User>();

        public string Name { get; set; }

        public void JoinUser(string userName)
        {
            var user = new User { Name = userName };
            Users.Add(user);

            OnUserJoining?.Invoke(this, new UserJoiningEventArg { RoomName = Name, UserName = user.Name });
            OnUserJoining += user.PrintSomeoneJoined;
            OnUserLeaving += user.PrintSomeoneLeft;
        }

        public void LeaveUser(string userName)
        {
            var otherUser = Users
                .Where(e => e.Name == userName)
                .FirstOrDefault();

            if (otherUser is null)
            {
                return;
            }

            Users.Remove(otherUser);
 
            OnUserJoining -= otherUser.PrintSomeoneJoined;
            OnUserLeaving -= otherUser.PrintSomeoneLeft;

            OnUserLeaving?.Invoke(this, new UserJoiningEventArg { RoomName = Name, UserName = otherUser.Name });
        }

        public void PrintUsers()
        {
            foreach(var user in Users)
            {
                Console.WriteLine(user.Name);
            }
        }
    }

    public class User
    {
        public string Name { get; set; }

        public void PrintSomeoneJoined(object sender, UserJoiningEventArg arg)
        {
            Console.WriteLine($"{Name}>>>>>{arg.UserName} joined the {arg.RoomName} room!");
        }

        public void PrintSomeoneLeft(object sender, UserJoiningEventArg arg)
        {
            Console.WriteLine($"{Name}>>>>>{arg.UserName} left the {arg.RoomName} room!");
        }
    }
}
